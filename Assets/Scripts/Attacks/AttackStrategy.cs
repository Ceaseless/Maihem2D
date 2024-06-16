using System.Collections.Generic;
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

        [SerializeField] public GameObject attackAnimation;
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
        public abstract IList<Vector2Int> GetAffectedTiles(Vector2Int position, Vector2Int direction, bool isPlayerAttack);
        public abstract IList<Vector2Int> GetPossibleTiles(Vector2Int position);
        
        protected bool TryDamage(Vector2Int target, int damage, bool isPlayerAttack)
        {
            if (isPlayerAttack)
            {
                if (GameManager.Instance.TryGetActorOnCell(target, out var actor))
                {
                    actor.TakeDamage(Damage);
                    return true;
                }
            }
            else
            {
                if (GameManager.Instance.CellContainsPlayer(target))
                {
                    GameManager.Instance.Player.TakeDamage(Damage);
                    return true;
                }
            }

            return false;
        }

        public abstract int getRange();

    }
}
