using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maihem.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Maihem
{
    public abstract class MovementStrategy : ScriptableObject
    {
        [SerializeField] private int alertRange;
        public Vector2Int IdleMove(Vector2Int gridPosition, Random randomGenerator)
        {
            var neighbours = MapManager.GetNeighbourPositions(gridPosition);

            foreach (var neighbour in neighbours.ToList())
            {
                if (MapManager.Instance.IsCellBlocking(neighbour) || MapManager.Instance.IsCellBlockedDiagonal(neighbour,gridPosition))
                {
                    neighbours.Remove(neighbour);
                }
            }

            var randomNumber = randomGenerator.Next(0, neighbours.Count - 1);
            return neighbours[randomNumber];
        }
        
        public bool CheckAlert(Vector2Int gridPosition)
        {
            var player = GameManager.Instance.Player;
            var distance = Math.Abs(player.GridPosition.x - gridPosition.x) + Math.Abs(player.GridPosition.y - gridPosition.y);
            if (distance <= alertRange)
            {
                return true;
            }

            return false;
        }

        public abstract Vector2Int ActivatedMove(Vector2Int gridPosition);
    }
}
