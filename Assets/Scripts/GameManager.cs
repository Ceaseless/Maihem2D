using TMPro;
using UnityEngine;

namespace Maihem
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private PlayerActor player;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private TextMeshProUGUI debugText;

        public int TurnCount { get; private set; }
        public PlayerActor Player => player;
        

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

        public bool TryGetActorOnCell(Vector2Int cellPosition, out Actor actor)
        {
            if (player.GridPosition == cellPosition)
            {
                actor = player;
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
            return player.GridPosition == cellPosition || enemyManager.CellContainsEnemy(cellPosition);
        }

        public bool CellContainsEnemy(Vector2Int cellPosition)
        {
            return enemyManager.CellContainsEnemy(cellPosition);
        }

        public bool CanTakeTurn()
        {
            return enemyManager.AreAllActionsPerformed() && !player.IsPerformingAction;
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
