using System.Collections.Generic;
using Maihem.Effects;
using Maihem.Managers;
using UnityEngine;


namespace Maihem.Attacks
{

    public abstract class AttackStrategy : ScriptableObject
    {
        [Header("General Settings")]
        [SerializeField] private string displayName;
        [Min(0)]
        [SerializeField] private int damage;
        [Min(0)]
        [SerializeField] private int staminaCost;

        [Header("General Effects")]
        [SerializeField] private VisualEffectSettings attackVisualEffect;
        [SerializeField] private AudioClip attackSoundEffect;
        
        [Header("On Hit Effects")]
        [SerializeField] private VisualEffectSettings onHitVisualEffect;
        [SerializeField] private AudioClip onHitSoundEffect;
        
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

            if (!hitSomething)
            {
                if (attackVisualEffect is not null)
                {
                    VisualEffectsPool.Instance.PlayVisualEffect(attackVisualEffect, MapManager.Instance.CellToWorld(target));
                }
            }
            else
            {
                PlayOnHitEffects(target);
            } 
            
            return hitSomething;
        }
        
        private void PlayOnHitEffects(Vector2Int tile)
        {
            var worldPosition = MapManager.Instance.CellToWorld(tile);
            if (onHitVisualEffect is not null)
            {
                VisualEffectsPool.Instance.PlayVisualEffect(onHitVisualEffect, MapManager.Instance.CellToWorld(tile));
            }
            
            if (onHitSoundEffect is not null)
            {
                AudioManager.Instance.PlaySoundFX(onHitSoundEffect, worldPosition, 1f);
            }
        }
        
        

        

    }
    
}
