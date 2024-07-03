using System.Collections.Generic;
using Maihem.Effects;
using Maihem.Managers;
using UnityEngine;


namespace Maihem.Attacks
{

    public abstract class AttackStrategy : ScriptableObject
    {
        [SerializeField] private string displayName;
        [Min(0)]
        [SerializeField] private int damage;
        [Min(0)]
        [SerializeField] private int staminaCost;

        [SerializeField] private VisualEffectSettings visualEffect;
        [SerializeField] private AudioClip hitSfx;
        public int Damage => damage;
        public int StaminaCost => staminaCost;
        
        public string DisplayName => displayName;

        protected static readonly Vector2Int[] AllDirections =
        {
            Vector2Int.up, 
            Vector2Int.down, 
            Vector2Int.left, 
            Vector2Int.right, 
            new(1,1),
            new(-1,1),
            new(1,-1),
            new(-1,-1),
        };
        
        public abstract bool Attack(Vector2Int position, Vector2Int direction, bool isPlayerAttack);
        public abstract IList<(Vector2Int, int)> GetAffectedTiles(Vector2Int position, Vector2Int direction, bool isPlayerAttack);
        public abstract IList<Vector2Int> GetPossibleTiles(Vector2Int position);
        public abstract int GetRange();
        protected bool TryDamage(Vector2Int target, int adjustedDamage, bool isPlayerAttack)
        {
            var hitSomething = false;
            if (isPlayerAttack)
            {
                if (GameManager.Instance.TryGetActorOnCell(target, out var actor))
                {
                    actor.healthSystem.TakeDamage(adjustedDamage);
                    hitSomething = true;
                }
            }
            else
            {
                if (GameManager.Instance.CellContainsPlayer(target))
                {
                    GameManager.Instance.Player.healthSystem.TakeDamage(adjustedDamage);
                    hitSomething = true;
                }
            }
            
            PlayHitVisualEffect(target);
            if(hitSomething) AudioManager.Instance.PlaySoundFX(hitSfx, target, 1f); 
            return hitSomething;
        }
        
        private void PlayHitVisualEffect(Vector2Int tile)
        {
            VisualEffectsPool.Instance.PlayVisualEffect(visualEffect, MapManager.Instance.CellToWorld(tile));
        }

        

    }
    
}
