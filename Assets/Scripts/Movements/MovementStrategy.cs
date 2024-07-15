using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Maihem.Movements
{
    public abstract class MovementStrategy : ScriptableObject
    {
        [SerializeField] private float alertRange;
        
        public static bool TryIdleMove(Vector2Int gridPosition, Vector2Int facing, out List<Vector2Int> path)
        {
            var cellInFacingDir = gridPosition + facing;
            if (!MapManager.Instance.IsCellBlocking(cellInFacingDir) &&
                !MapManager.Instance.IsCellBlockedDiagonal(cellInFacingDir, gridPosition) &&
                !GameManager.Instance.CellContainsActor(cellInFacingDir))
            {
                path = new List<Vector2Int> { cellInFacingDir };
                return true;
            }

            path = null;
            return false;
        }
        
        public bool CheckAlert(Vector2Int gridPosition, Vector2Int facingVector)
        {
            var player = GameManager.Instance.Player;
            var distanceToPlayer = Vector2Int.Distance(player.GridPosition, gridPosition);
            if (distanceToPlayer > alertRange) return false; // Out of detection range => don't continue
            
            var enemyToPlayer = player.transform.position - MapManager.Instance.CellToWorld(gridPosition);
            var vecFacing = new Vector3(facingVector.x, facingVector.y, 0);
            var dot = Vector3.Dot(enemyToPlayer, vecFacing);
            return dot >= 0; // Facing towards player (180° field of vision)
        }

        public abstract List<Vector2Int> ActivatedMove(Vector2Int gridPosition, int range);
    }
}
