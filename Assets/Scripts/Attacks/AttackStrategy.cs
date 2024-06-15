using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Attacks
{

    public abstract class AttackStrategy : ScriptableObject
    {
        [Min(0)]
        [SerializeField] private int damage;
        [Min(0)]
        [SerializeField] private int staminaCost;

        [SerializeField] public GameObject attackAnimation;
        public int Damage => damage;
        public int StaminaCost => staminaCost;

        public abstract bool Attack(Vector2Int position, Vector2Int direction, bool isPlayerAttack);
        public abstract IList<Vector2Int> GetAffectedTiles(Vector2Int position, Vector2Int direction, bool isPlayerAttack);
        public abstract IList<Vector2Int> GetPossibleTiles(Vector2Int position);
        
        protected bool TryDamage(Vector2Int target, bool isPlayerAttack)
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

    }
}
