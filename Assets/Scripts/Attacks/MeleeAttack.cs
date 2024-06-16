using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Attacks
{
    [CreateAssetMenu(menuName = "Attack Strategies/Melee Attack", order = 0)]
    public class MeleeAttack : AttackStrategy
    {
        public override bool Attack(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            var target = position + direction;
            return TryDamage(target,Damage, isPlayerAttack);
        }

        public override IList<Vector2Int> GetAffectedTiles(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            return new List<Vector2Int>() { position + direction };
        }

        public override IList<Vector2Int> GetPossibleTiles(Vector2Int position)
        {
            return MapManager.GetNeighbourPositions(position);
        }
    }
}