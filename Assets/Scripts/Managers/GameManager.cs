using System.Collections.Generic;
using Cinemachine;
using Maihem.Actors;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Maihem.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Vector3 playerStartPosition;
        [SerializeField] private KillZoneController boundsController;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private PickupManager pickupManager;
        [FormerlySerializedAs("ui")] [SerializeField] private UIManager uiManager;
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private CinemachineVirtualCamera followCamera;

        public int TurnCount { get; private set; }
        public PlayerActor Player { get; private set; }

        private static bool _gameOver;

        
        

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
        }

        private void SpawnPlayer()
        {
            if (Player)
            {
                Destroy(Player.gameObject);
            }

            var playerObject = Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
            Player = playerObject.GetComponent<PlayerActor>();
            Player.Initialize();
            followCamera.Follow = Player.transform;
        }

        public void ResetGame()
        {
            enemyManager.Reset();
            pickupManager.Reset();
            boundsController.Reset();
            MapManager.Instance.Reset();
            MarkerPool.Instance.HideAllMarkers();
            TurnCount = 0;
            SpawnPlayer();
            _gameOver = false;
            uiManager.Initialize();
            
            debugText.text = $"Turn: {TurnCount}";
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
            return enemyManager.AreAllActionsPerformed() && !Player.IsPerformingAction;
        }

        public IList<Enemy> GetEnemiesInProximity(Vector2Int origin ,int range)
        {
            return enemyManager.GetEnemiesInProximity(origin, range);
        }
    
        public void TriggerTurn()
        {
            if (_gameOver) return;
            boundsController.UpdateBounds();
            if (Player.transform.position.x <= boundsController.transform.position.x)
            {
                Debug.Log("Player walked into the light!");
                ResetGame();
                return;
            }
            
            pickupManager.CullUsedPickups();
            enemyManager.Tick();
            MapManager.Instance.UpdateMap();
            TurnCount++;
            UpdateUI();
            
            if (!Player.IsDead) return;
            Debug.Log("Player died");
            ResetGame();
        }
    
        private void UpdateUI()
        {
            debugText.text = $"Turn: {TurnCount}";
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
