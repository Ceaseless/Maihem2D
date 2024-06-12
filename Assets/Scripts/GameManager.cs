using TMPro;
using UnityEngine;

namespace Maihem
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Vector3 playerStartPosition;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private UIManager ui;
        [SerializeField] private TextMeshProUGUI debugText;

        public int TurnCount { get; private set; }
        public PlayerActor Player { get; private set; }

        public UIManager UI => ui;
        

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
                return;
            }
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            if (Player)
            {
                Destroy(Player.gameObject);
            }

            var playerObject = Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
            Player = playerObject.GetComponent<PlayerActor>();
            ui.SetupPlayer(Player.CurrentHealth,Player.GetMaxStamina(), Player.GridPosition);
        }

        public void ResetGame()
        {
            SpawnPlayer();
            enemyManager.Reset();
            cameraController.Reset();
            TurnCount = 0;
            ui.UpdateStatus();
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
            cameraController.UpdateCameraScroll();
            if (cameraController.IsPositionOffScreen(Player.transform.position))
            {
                Debug.Log("Player walked off screen!");
                ResetGame();
                return;
            }
            
            enemyManager.Tick();
            MapManager.Instance.UpdateMap();
            TurnCount++;
            ui.UpdatePlayer(Player.CurrentHealth,Player.GetStamina(), Player.GridPosition);
            UpdateUI();
            
            if (!Player.IsDead) return;
            Debug.Log("Player died");
            ResetGame();
        }
    
        private void UpdateUI()
        {
            debugText.text = $"Turn: {TurnCount}";
        }

   
    }
}
