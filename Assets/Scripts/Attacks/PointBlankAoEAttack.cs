﻿using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem.Attacks
{
    [CreateAssetMenu(menuName = "Attack Strategies/Point Blank AoE Attack")]
    public class PointBlankAoEAttack : AttackStrategy
    {
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

        private int CalculateTileDamage(int distance)
        {
            return math.max(0,
                invertDamageFalloff
                    ? Damage - (range - distance) * damageFalloff
                    : Damage - damageFalloff * (distance - 1));
        }

        public override IList<(Vector2Int,int)> GetAffectedTiles(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            var targets = new List<(Vector2Int,int)>();
            for (var x = -range; x <= range; x++)
            {
                for (var y = -range; y <= range; y++)
                {
                    if(x == 0 && y== 0 || math.abs(x) + math.abs(y) > range) continue;
                    
                    var targetPosition = position+new Vector2Int(x, y);
                    var dist = math.max(math.abs(x), math.abs(y));
                    var adjustedDamage = CalculateTileDamage(dist);
                    targets.Add((targetPosition, adjustedDamage));
                }
            }

            return targets;
        }

        public override IList<Vector2Int> GetPossibleTiles(Vector2Int position)
        {
            var targets = new List<Vector2Int>();
            for (var x = -range; x <= range; x++)
            {
                for (var y = -range; y <= range; y++)
                {
                    if(x == 0 && y== 0 || math.abs(x) + math.abs(y) > range) continue;
                    targets.Add((position+new Vector2Int(x,y)));
                }
            }

            return targets;
        }
        public override int GetRange()
        {
            return 0;
        }
    }
}