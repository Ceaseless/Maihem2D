using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Attacks
{
    [CreateAssetMenu(menuName = "Attack Strategies/Ranged Attack")]
    public class RangedAttack : AttackStrategy
    {
        [Header("Ranged Attack Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private float projectileTime;
        [SerializeField] private int range;
        [SerializeField] private bool blockedByActors;
        public override bool Attack(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            
            var lineTiles = GetAffectedTiles(position, direction, isPlayerAttack);
            foreach (var (target, damage) in lineTiles)
            {
                if (TryDamage(target, damage, isPlayerAttack))
                {
                    Instantiate(projectilePrefab).GetComponent<ProjectileEffect>().LaunchFromTo(MapManager.Instance.CellToWorld(position), MapManager.Instance.CellToWorld(target), projectileTime);
                    return true;
                }
            }

            var lastTarget = lineTiles[^1];
            Instantiate(projectilePrefab).GetComponent<ProjectileEffect>().LaunchFromTo(MapManager.Instance.CellToWorld(position), MapManager.Instance.CellToWorld(lastTarget.Item1), projectileTime);
            
            return false;
        }
     

        public override IList<(Vector2Int,int)> GetAffectedTiles(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            var map = MapManager.Instance;
            var game = GameManager.Instance;
            for (var i = 1; i <= range; i++)
            {
                var tilePosition = position + direction * i;
                if (map.IsCellBlocking(tilePosition)) return new List<(Vector2Int,int)>{ (tilePosition, Damage) };
                if (blockedByActors && game.CellContainsActor(tilePosition)) return new List<(Vector2Int,int)>{ (tilePosition, Damage) };
                switch (isPlayerAttack)
                {
                    case true when game.CellContainsEnemy(tilePosition):
                        return new List<(Vector2Int,int)>{ (tilePosition, Damage) };
                    case false when game.CellContainsPlayer(tilePosition):
                        return new List<(Vector2Int,int)>{ (tilePosition, Damage) };
                }
            }
            return new List<(Vector2Int,int)>() { (position + direction * range, Damage) };
        }

        public override IList<Vector2Int> GetPossibleTiles(Vector2Int position)
        {
            var tiles = new List<Vector2Int>();
            

            foreach (var direction in AllDirections)
            {
                for (var i = 1; i <= range; i++)
                {
                    var map = MapManager.Instance;
                    var tilePosition = position + direction * i;
                    tiles.Add(tilePosition);
                    if (map.IsCellBlocking(tilePosition)) break;
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