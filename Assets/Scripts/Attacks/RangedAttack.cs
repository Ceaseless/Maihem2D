using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Attacks
{
    [CreateAssetMenu(menuName = "Attack Strategies/Ranged Attack")]
    public class RangedAttack : AttackStrategy
    {
        [SerializeField] private int range;
        [SerializeField] private bool blockedByActors;
        public override bool Attack(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            var lineTiles = GetAffectedTiles(position, direction, isPlayerAttack);
            foreach (var tile in lineTiles)
            {
                if (TryDamage(tile, isPlayerAttack)) return true;
            }

            return false;
        }

        public override IList<Vector2Int> GetAffectedTiles(Vector2Int position, Vector2Int direction, bool isPlayerAttack)
        {
            var map = MapManager.Instance;
            var game = GameManager.Instance;
            for (var i = 1; i <= range; i++)
            {
                var tilePosition = position + direction * i;
                if (map.IsCellBlocking(tilePosition)) return new List<Vector2Int>{ tilePosition };
                if (blockedByActors && game.CellContainsActor(tilePosition)) return new List<Vector2Int>{ tilePosition };
                switch (isPlayerAttack)
                {
                    case true when game.CellContainsEnemy(tilePosition):
                        return new List<Vector2Int>{ tilePosition };
                    case false when game.CellContainsPlayer(tilePosition):
                        return new List<Vector2Int>{ tilePosition };
                }
            }
            return new List<Vector2Int>() { position + direction * range };
        }

        public override IList<Vector2Int> GetPossibleTiles(Vector2Int position)
        {
            var tiles = new List<Vector2Int>();
            Vector2Int[] allDirections =
            {
                Vector2Int.up, 
                Vector2Int.down, 
                Vector2Int.left, 
                Vector2Int.right, 
                new(1,1),
                new(-1,1),
                new(1,-1),
                new(-1,-1),
            };

            foreach (var direction in allDirections)
            {
                for (var i = 1; i <= range; i++)
                {
                    tiles.Add(position+direction*i);
                }
            }
            return tiles;
        }
    }
}