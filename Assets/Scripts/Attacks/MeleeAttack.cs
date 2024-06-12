using System.Collections.Generic;
using UnityEngine;

namespace Maihem.Attacks
{
    [CreateAssetMenu(menuName = "Attack Strategies/Melee Attack", order = 0)]
    public class MeleeAttack : AttackStrategy
    {
        public override void Attack(Vector2Int position, Vector2Int direction)
        {
            var target = position + direction;
            if (GameManager.Instance.TryGetActorOnCell(target, out var actor))
            {
                actor.TakeDamage(Damage);
            }
        }

        public override IList<Vector2Int> GetAffectedTiles(Vector2Int position, Vector2Int direction)
        {
            return new List<Vector2Int>() { position + direction };
        }
    }
}