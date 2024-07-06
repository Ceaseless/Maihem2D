using System;
using System.Collections.Generic;
using Maihem.Managers;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem.Attacks
{
    [CreateAssetMenu(menuName = "Attack Strategies/Smash Attack")]
    public class SmashAttack : AttackStrategy
    {
        [Header("Smash Settings")]
        [Min(1)]
        [SerializeField] private int range = 3;
        [Min(1)]
        [SerializeField] private int width = 1;
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
            var map = MapManager.Instance;
            var playerPosition = GameManager.Instance.Player.GridPosition;
            var difX = Mathf.Abs(position.x - playerPosition.x);
            var difY = Mathf.Abs(position.y - playerPosition.y);
            
            if (direction.x != 0 && difX>difY)
            {
                for (var y = -width; y <= width; y++)
                {
                    for (var x = direction.x; x <= range && x >= -range; x += direction.x)
                    {
                        var targetPosition = position+new Vector2Int(x, y);
                        if (map.IsCellBlocking(targetPosition)) break;
                        var dist = math.max(math.abs(x), math.abs(y));
                        var adjustedDamage = CalculateTileDamage(dist);
                        targets.Add((targetPosition, adjustedDamage));
                    }
                }
            }
            else
            {
                for (var x = -width; x <= width; x++)
                {
                    for (var y = direction.y; y <= range && y >= -range; y += direction.y)
                    {
                        var targetPosition = position+new Vector2Int(x, y);
                        if (map.IsCellBlocking(targetPosition)) break;
                        var dist = math.max(math.abs(x), math.abs(y));
                        var adjustedDamage = CalculateTileDamage(dist);
                        targets.Add((targetPosition, adjustedDamage));
                    }
                }
            }
            return targets;
        }

        public override IList<Vector2Int> GetPossibleTiles(Vector2Int position)
        {
            var targets = new List<Vector2Int>();
            var map = MapManager.Instance;
            foreach (var direction in AllDirections)
            {
                if (direction.x != 0)
                {
                    for (var y = -width; y <= width; y++)
                    {
                        for (var x = direction.x; x <= range && x >= -range; x += direction.x)
                        {
                            var targetPosition = position + new Vector2Int(x, y);
                            if (map.IsCellBlocking(targetPosition)) break;
                            targets.Add(targetPosition);
                        }
                    }
                }
                if(direction.y != 0)
                {
                    for (var x = -width; x <= width; x++)
                    {
                        for (var y = direction.y; y <= range && y >= -range; y += direction.y)
                        {
                            var targetPosition = position + new Vector2Int(x, y);
                            if (map.IsCellBlocking(targetPosition)) break;
                            targets.Add(targetPosition);
                        }
                    }
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