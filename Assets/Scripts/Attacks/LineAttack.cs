﻿using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem.Attacks
{
    [CreateAssetMenu(menuName = "Attack Strategies/Line Attack")]
    public class LineAttack : AttackStrategy
    {
        [Min(1)]
        [SerializeField] private int range = 3;
        [Min(0)]
        [SerializeField] private int damageFalloff;
        [SerializeField] private bool invertDamageFalloff;
        
        protected static readonly int AnimatorAttack = Animator.StringToHash("Slam");
        
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
                var adjustedDamage = math.max(0, invertDamageFalloff ? Damage - (range-i) * damageFalloff : Damage - damageFalloff * (i-1));
                targets.Add((position+direction*i, adjustedDamage));
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
                    tiles.Add(position+direction*i);
                }
            }
            return tiles;
        }
        
        public override int GetRange()
        {
            return 0;
        }
    }
}