using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Attacks
{
    [CreateAssetMenu(menuName = "Attack Strategies/Melee Attack", order = 0)]
    public class MeleeAttack : AttackStrategy
    {
        [SerializeField] private GameObject hitEffect;
        public override void Attack(Vector2Int position, Vector2Int direction)
        {
            var target = position + direction;
            Instantiate(hitEffect, MapManager.Instance.CellToWorld(target),Quaternion.identity);
            if (GameManager.Instance.TryGetActorOnCell(target, out var actor))
            {
                actor.TakeDamage(Damage);
            }
        }

        public override IList<Vector2Int> GetAffectedTiles(Vector2Int position, Vector2Int direction)
        {
            return new List<Vector2Int>() { position + direction };
        }

        public override IList<Vector2Int> GetPossibleTiles(Vector2Int position)
        {
            return MapManager.GetNeighbourPositions(position);
        }
    }
}