using Maihem.Effects;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Pickups
{
    public abstract class Pickup : MonoBehaviour
    {
        [Header("On Pickup Effects")]
        [SerializeField] protected AudioClip soundEffect;
        [SerializeField] protected VisualEffectSettings visualEffect;

        [Header("Settings")]
        [SerializeField] public float spawnChance;
        
        public bool IsUsed { get; set; }
        public float SpawnChance => spawnChance;
        private Vector2Int GridPosition { get; set; }
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            SnapToGrid();
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

        protected void PlayOnPickUpEffects()
        {
            if (visualEffect is not null)
            {
                VisualEffectsPool.Instance.PlayVisualEffect(visualEffect, transform.position);
            }
            if (soundEffect is not null)
            {
                AudioManager.Instance.PlaySoundFX(soundEffect, transform.position, 1f);
            }
        }

        protected abstract void OnPickUp();
    }
}
