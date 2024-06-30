using Maihem.Managers;
using UnityEngine;

namespace Maihem.Pickups
{
    public abstract class Pickup : MonoBehaviour
    {

        [SerializeField] public float spawnChance;
        [SerializeField] protected Color pickupColor;
        [SerializeField] protected SpriteRenderer spriteRenderer;

        public bool IsUsed { get; protected set; }
        public float SpawnChance => spawnChance;
        private Vector2Int GridPosition { get; set; }
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            SnapToGrid();
            spriteRenderer.color = pickupColor;
        }

        private void SnapToGrid()
        {
            var cellPosition = MapManager.Instance.WorldToCell(transform.position);
            GridPosition = cellPosition;
            var newPosition = MapManager.Instance.CellToWorld(GridPosition);
            transform.position = newPosition;
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsUsed) return;
            OnPickUp();
        }

        protected abstract void OnPickUp();
    }
}
