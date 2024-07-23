using System;
using System.Collections.Generic;
using Cinemachine;
using Maihem.Actors;
using Maihem.Maps;
using Maihem.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maihem.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private KillZoneController boundsController;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private PickupManager pickupManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private CinemachineVirtualCamera followCamera;
        [SerializeField] private CinemachineConfiner2D cameraConfiner;
        [SerializeField] private AudioClip playerLostSound;
        
        private int TurnCount { get; set; }
        public Player Player { get; private set; }
        public PlayerInput PlayerInput => playerInput;

        private bool _playerLost;
        private bool _gameOver;
        private bool _triggerTurnOnNextFrame;
        private bool _nonPlayerTurn;
        private bool _performPostSceneChangeSetup;
        


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            
        }

        private void Start()
        {
            enemyManager.Initialize();
            pickupManager.Initialize();
            MapManager.Instance.Initialize();
            SpawnPlayer();
            _gameOver = false;
            _playerLost = false;
            uiManager.Initialize();
            enemyManager.AllEnemiesPerformedTurn = OnEnemyTurnCompleted;
            enemyManager.UpdateEnemiesActiveState();
            pickupManager.UpdatePickupsActiveState();
            audioManager.FadeInMusic(2f);
            _performPostSceneChangeSetup = true;
            PlayerInput.PauseGameAction += PauseGame;

        }

        private void OnDestroy()
        {
            PlayerInput.PauseGameAction -= PauseGame;
        }

        private void SpawnPlayer()
        {
            if (Player)
            {
                Player.TurnCompleted -= OnPlayerTurnComplete;
                Destroy(Player.gameObject);
            }

            var playerObject = Instantiate(playerPrefab, MapManager.Instance.GetStartPosition(), Quaternion.identity);
            Player = playerObject.GetComponent<Player>();
            Player.Initialize();
            Player.TurnCompleted += OnPlayerTurnComplete;
            
            followCamera.Follow = Player.transform;
            
            
        }

        public void ResetGame()
        {
            _triggerTurnOnNextFrame = false;
            enemyManager.ResetState();
            pickupManager.ResetState();
            boundsController.ResetState();
            MapManager.Instance.ResetState();
            MarkerPool.Instance.HideAllMarkers();
            TurnCount = 0;
            SpawnPlayer();
            _gameOver = false;
            _nonPlayerTurn = false;
            _playerLost = false;
            enemyManager.UpdateEnemiesActiveState();
            pickupManager.UpdatePickupsActiveState();
            uiManager.ResetState();
            audioManager.ResetMusic();
            
        }

        public void PassMapData(MapData data)
        {
            enemyManager.RegisterEnemies(data.MapEnemies);
            pickupManager.RegisterPickups(data.MapPickups);
        }

        public bool TryGetActorOnCell(Vector2Int cellPosition, out Actor actor)
        {
            if (Player.GridPosition == cellPosition)
            {
                actor = Player;
                return true;
            }

            if (enemyManager.TryGetEnemyOnCell(cellPosition, out var enemy))
            {
                actor = enemy;
                return true;
            }
            actor = null;
            return false;
        }

        public bool TryGetEnemyOnCell(Vector2Int cellPosition, out Enemy foundEnemy)
        {
            if (enemyManager.TryGetEnemyOnCell(cellPosition, out var enemy))
            {
                foundEnemy = enemy;
                return true;
            }
            foundEnemy = null;
            return false;
        }

        public bool CellContainsPlayer(Vector2Int cellPosition)
        {
            return Player.GridPosition == cellPosition;
        }

        public bool CellContainsActor(Vector2Int cellPosition)
        {
            return Player.GridPosition == cellPosition || enemyManager.CellContainsEnemy(cellPosition);
        }

        public bool CellContainsEnemy(Vector2Int cellPosition)
        {
            return enemyManager.CellContainsEnemy(cellPosition);
        }

        public bool CanTakeTurn()
        {
            return !_playerLost && !_nonPlayerTurn && !_triggerTurnOnNextFrame && enemyManager.AreAllActionsPerformed() && !Player.IsPerformingAction;
        }

        public IList<Enemy> GetEnemiesInProximity(Vector2Int origin ,int range)
        {
            return enemyManager.GetEnemiesInProximity(origin, range);
        }

        private void OnPlayerTurnComplete(object sender, EventArgs args)
        {
            _nonPlayerTurn = true;
            _triggerTurnOnNextFrame = true;
        }

        
        private void Update()
        {
            if (_performPostSceneChangeSetup)
            {
                cameraConfiner.InvalidateCache();
                followCamera.Follow = Player.transform;
                _performPostSceneChangeSetup = false;
            }
            
            if (_gameOver || !_triggerTurnOnNextFrame) return;
            TriggerTurn();
        }

        private void TriggerTurn()
        {
            _triggerTurnOnNextFrame = false;
            boundsController.UpdateBounds();
            if (Player.transform.position.x <= boundsController.transform.position.x)
            {
                uiManager.ShowGameOverUI(GameOverUI.GameOverReason.Light);
                _playerLost = true;
                GameOver();
                return;
            }
            pickupManager.Tick();
            enemyManager.Tick();
        }
        

        private void OnEnemyTurnCompleted()
        {
            MapManager.Instance.UpdateMap();
            Player.Tick();
            TurnCount++;
            _nonPlayerTurn = false;
            
            if (!Player.IsDead) return;
            uiManager.ShowGameOverUI(GameOverUI.GameOverReason.Health);
            _playerLost = true;
            GameOver();
        }

        public void PauseGame(object sender, EventArgs args)
        {
            if (!CanTakeTurn() || _gameOver) return;
            var toggleValue = !Player.IsPaused;
            if (uiManager.TrySetPauseMenuActive(toggleValue))
            {
                Player.PausePlayer(toggleValue);
            }
        }

        public void LoadMainMenu()
        {
            PlayerInput.PauseGameAction -= PauseGame;
            SceneLoadingData.SceneToLoad = SceneLoadingData.LoadableScene.MainMenu;
            SceneManager.LoadScene(SceneLoadingData.LoadableScene.LoadingScene.ToString());
        }
        
        public void Exit()
        {
            Application.Quit();
        }

        public void GameOver()
        {
            _gameOver = true;
            if (_playerLost)
            {
                AudioManager.Instance.StopMusic();
                AudioManager.Instance.PlaySoundFX(playerLostSound,Player.GridPosition,1f);
            }
            uiManager.ShowGameOverUI(GameOverUI.GameOverReason.Win);
        }

        public void GoalReached()
        {
            Player.PausePlayer(true);
            Player.gameObject.SetActive(false);
        }

        public void ToggleKillZone(bool active)
        {
            boundsController.stopped = active;
        }

        public void TogglePlayerStats(bool active)
        {
            uiManager.TogglePlayerStats(active);
        }

        public void ItemButtonFlash(string color)
        {
            uiManager.ItemButtonFlash(color);
        }
        public void AttackButtonFlash(string color)
        {
            uiManager.AttackButtonFlash(color);
        }
        
    }
}
