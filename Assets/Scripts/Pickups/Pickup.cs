using Maihem.Managers;
using UnityEngine;

namespace Maihem.Pickups
{
    public abstract class Pickup : MonoBehaviour
    {

        [SerializeField] private float spawnChance;
        [SerializeField] private Color pickupColor;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public bool IsUsed { get; private set; }
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


        public void OnTriggerEnter2D(Collider2D other)
        {
            OnPickUp();
            IsUsed = true;
        }

        protected abstract void OnPickUp();
    }
}
