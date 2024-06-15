using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    public abstract class Pickup : MonoBehaviour
    {
        
        public Vector2Int GridPosition { get; set; }
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            SnapToGrid();
        }

        protected void SnapToGrid()
        {
            var cellPosition = MapManager.Instance.WorldToCell(transform.position);
            GridPosition = cellPosition;
            var newPosition = MapManager.Instance.CellToWorld(GridPosition);
            transform.position = newPosition;
        }

        public abstract void PickUp();
    }
}
