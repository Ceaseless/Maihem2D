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
        [SerializeField] private TextMeshProUGUI debugText;

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
                return;
            }
            StartGame();
        }

        private void StartGame()
        {
            if (Player)
            {
                Destroy(Player.gameObject);
            }

            var playerObject = Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
            Player = playerObject.GetComponent<PlayerActor>();
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
            enemyManager.Tick();
            MapManager.Instance.UpdateMap();
            TurnCount++;
            UpdateUI();
        }
    
        private void UpdateUI()
        {
            debugText.text = $"Turn: {TurnCount}";
        }

   
    }
}
