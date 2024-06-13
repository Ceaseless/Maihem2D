using System;
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
        [FormerlySerializedAs("ui")] [SerializeField] private UIManager uiManager;
        [SerializeField] private TextMeshProUGUI debugText;
        [SerializeField] private CinemachineVirtualCamera followCamera;

        public int TurnCount { get; private set; }
        public PlayerActor Player { get; private set; }

        
        

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
            followCamera.Follow = Player.transform;
        }

        public void ResetGame()
        {
            SpawnPlayer();
            enemyManager.Reset();
            boundsController.Reset();
            TurnCount = 0;
            uiManager.Initialize();
            
            debugText.text = $"Turn: {TurnCount}";
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
    
        public void TriggerTurn()
        {
            boundsController.UpdateBounds();
            if (Player.transform.position.x <= boundsController.transform.position.x)
            {
                Debug.Log("Player walked into the light!");
                ResetGame();
                return;
            }
            
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
            uiManager.UpdateStatusUI();
        }

   
    }
}
