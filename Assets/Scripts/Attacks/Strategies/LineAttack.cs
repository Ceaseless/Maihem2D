using System.Collections.Generic;
using Maihem.Managers;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem.Attacks.Strategies
{
    [CreateAssetMenu(menuName = "Attack Strategies/Line Attack")]
    public class LineAttack : AttackStrategy
    {
        [Header("Line Attack Settings")]
        [Min(1)]
        [SerializeField] private int range = 3;
        [Min(0)]
        [SerializeField] private int damageFalloff;
        [SerializeField] private bool invertDamageFalloff;

        public override bool Attack(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            var targets = GetAffectedTiles(position, direction, isPlayerAttack);
            var hitSomething = false;

            foreach (var (target, damage) in targets)
            {
                hitSomething = TryDamage(target, damage, isPlayerAttack);
            }

            return hitSomething;
        }

        

        public override IList<(Vector2Int, int)> GetAffectedTiles(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            var targets = new List<(Vector2Int,int)>();
            for (var i = 1; i <= range; i++)
            {
                var targetPosition = position + direction * i;
                var adjustedDamage = math.max(0, invertDamageFalloff ? Damage - (range-i) * damageFalloff : Damage - damageFalloff * (i-1));
                targets.Add((targetPosition, adjustedDamage));
                if (MapManager.Instance.IsCellBlocking(targetPosition)) break;
                
            }
            return targets;
        }

        public override IList<Vector2Int> GetPossibleTiles(Vector2Int position)
        {
            var tiles = new List<Vector2Int>();
            
            foreach (var direction in AllDirections)
            {
                for (var i = 1; i <= range; i++)
                {
                    var targetPosition = position + direction * i;
                    tiles.Add(targetPosition);
                    if (MapManager.Instance.IsCellBlocking(targetPosition)) break;
                }
            }
            return tiles;
        }
        
        public override int GetRange()
        {
            return range;
        }
    }
}