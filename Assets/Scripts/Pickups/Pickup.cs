using System;
using Maihem.Actors;
using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    public abstract class Pickup : MonoBehaviour
    {

        [SerializeField] public float spawnChance;
        private Vector2Int GridPosition { get; set; }

        public bool Used { get; private set; }
        
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            SnapToGrid();
            Used = false;
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
            if (other.gameObject.GetComponent<PlayerActor>() == null || Used) return;
            PickUp();
            Used = true;
        }
        public abstract void PickUp();
    }
}
