﻿using System.Collections.Generic;
using Maihem.Managers;
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
        [SerializeField] private int damageFalloff = 0;
        
        public override void Attack(Vector2Int position, Vector2Int direction)
        {
            var targets = GetAffectedTiles(position, direction);
            foreach (var target in targets)
            {
                if (GameManager.Instance.TryGetActorOnCell(target, out var actor))
                {
                    actor.TakeDamage(Damage);
                }
            }
        }

        public override IList<Vector2Int> GetAffectedTiles(Vector2Int position, Vector2Int direction)
        {
            var targets = new List<Vector2Int>();
            for (var x = -range; x <= range; x++)
            {
                for (var y = -range; y <= range; y++)
                {
                    if(x == 0 && y== 0) continue;
                    if (math.abs(x)+math.abs(y) <= range)
                    {
                        targets.Add(position+new Vector2Int(x,y));
                    }
                }
            }

            return targets;
        }
    }
}