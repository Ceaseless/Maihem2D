using System;
using System.Collections.Generic;
using Cinemachine;
using Maihem.Actors;
using UnityEngine;

namespace Maihem.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Vector3 playerStartPosition;
        [SerializeField] private KillZoneController boundsController;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private PickupManager pickupManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private CinemachineVirtualCamera followCamera;

        private int TurnCount { get; set; }
        public Player Player { get; private set; }
        public PlayerInput PlayerInput => playerInput;

        private bool _gameOver;
        private bool _triggerTurnOnNextFrame;
        private bool _nonPlayerTurn;

        
        

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SpawnPlayer();
            _gameOver = false;
            uiManager.Initialize();
            enemyManager.AllEnemiesPerformedTurn = OnEnemyTurnCompleted;
            audioManager.FadeInMusic(2f);
        }

        private void SpawnPlayer()
        {
            if (Player)
            {
                Player.TurnCompleted -= OnPlayerTurnComplete;
                Destroy(Player.gameObject);
            }

            var playerObject = Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
            Player = playerObject.GetComponent<Player>();
            Player.Initialize();
            Player.TurnCompleted += OnPlayerTurnComplete;
            
            followCamera.Follow = Player.transform;
        }

        public void ResetGame()
        {
            _triggerTurnOnNextFrame = false;
            enemyManager.Reset();
            pickupManager.Reset();
            boundsController.Reset();
            MapManager.Instance.Reset();
            MarkerPool.Instance.HideAllMarkers();
            TurnCount = 0;
            SpawnPlayer();
            _gameOver = false;
            _nonPlayerTurn = false;
            uiManager.Initialize();
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
            return !_nonPlayerTurn && !_triggerTurnOnNextFrame && enemyManager.AreAllActionsPerformed() && !Player.IsPerformingAction;
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
            if (_gameOver || !_triggerTurnOnNextFrame) return;
            TriggerTurn();
        }

        private void TriggerTurn()
        {
            _triggerTurnOnNextFrame = false;
            boundsController.UpdateBounds();
            if (Player.transform.position.x <= boundsController.transform.position.x)
            {
                Debug.Log("Player walked into the light!");
                ResetGame();
                return;
            }
            pickupManager.CullUsedPickups();
            enemyManager.Tick();
        }
        

        private void OnEnemyTurnCompleted()
        {
            MapManager.Instance.UpdateMap();
            Player.Tick();
            TurnCount++;
            _nonPlayerTurn = false;
            
            
            if (!Player.IsDead) return;
            Debug.Log("Player died");
            ResetGame();
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void GameOver()
        {
            _gameOver = true;
            Player.PausePlayer();
            uiManager.ShowWinScreen();
        }
    }
}
