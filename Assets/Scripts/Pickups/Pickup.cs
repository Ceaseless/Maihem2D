using Maihem.Managers;
using UnityEngine;

namespace Maihem.Pickups
{
    public abstract class Pickup : MonoBehaviour
    {

        [SerializeField] public float spawnChance;
        [SerializeField] public Color pickupColor;

        private Vector2Int GridPosition { get; set; }

        

        public bool Used { get; private set; }
        
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            SnapToGrid();
            Used = false;
            GetComponentInChildren<Renderer>().material.color = pickupColor;
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
            if (Used || other.gameObject != GameManager.Instance.Player.gameObject) return;
            PickUp();
            Used = true;
        }

        protected abstract void PickUp();
    }
}
