﻿using System.Collections.Generic;
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
        
        public override bool Attack(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            var targets = GetAffectedTiles(position, direction, isPlayerAttack);
            var hitSomething = false;
 
            foreach (var target in targets)
            {
                hitSomething = TryDamage(target, Damage, isPlayerAttack);
            }

            return hitSomething;
        }

        

        public override IList<Vector2Int> GetAffectedTiles(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            var targets = new List<Vector2Int>();
            for (var i = 1; i <= range; i++)
            {
                targets.Add(position+direction*i);
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
        
        public override int getRange()
        {
            return 0;
        }
    }
}