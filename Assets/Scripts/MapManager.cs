using System;
using System.Collections.Generic;
using System.Linq;
using Maihem.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Maihem
{
    internal struct Node
    {
        public Vector2Int Position;

        public float G { get; private set; }
        public float H { get; private set; }
        public float F => G + H;

        public List<Node> Neighbors { get; set; }

        public Node[] Connection;

        public Node(Vector2Int position, Vector2Int start, Vector2Int goal)
        {
            Position = position;
            G = Math.Abs(goal.y-Position.y)+Math.Abs(goal.x-Position.x);
            H = Math.Abs(start.y-Position.y)+Math.Abs(start.x-Position.x);
            Neighbors = null;
            Connection = null;
        }

        public void SetG(float g) => G = g;

        public void SetH(float h) => H = h;

        public void SetConnection(Node connect) => Connection = new[] { connect };

        public float GetDistance(Vector2Int target)
        {
            return (Math.Abs(target.y-Position.y)+Math.Abs(target.x-Position.x)); 
        }

        public bool ContainsNode(Vector2Int identity)
        {
            foreach (var i in Neighbors)
            {
                if (i.Position == identity)
                {
                    return true;
                }
            }
            return false;
        }
    }
    
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }
        [SerializeField] private Grid grid;
        
        private List<Tilemap> _tilemaps;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _tilemaps = grid.GetComponentsInChildren<Tilemap>().ToList();
        }
        
        public void UpdateMap()
        {
            // TODO
        }

        public bool IsCellBlocking(Vector3 worldPosition)
        {
            var gridPosition = MapManager.Instance.GetGridPositionFromWorldPosition(worldPosition);
            return IsCellBlocking(gridPosition);
        }
    
        public bool IsCellBlocking(Vector2Int gridPosition)
        {
            var cellPosition = gridPosition.WithZ(0);
            
            if (!TryGetTilemapContainingCell(gridPosition, out var map)) return true;
            
            var cell = map.GetTile<Tile>(cellPosition);
            return cell is null || cell.colliderType != Tile.ColliderType.None;
        }

        public bool IsCellBlockedDiagonal(Vector2Int gridPosition, Vector2Int origin)
        {
            var moveVector = new Vector2Int(gridPosition.x - origin.x,gridPosition.y-origin.y);
            
            var moveX = new Vector2Int(moveVector.x, 0);
            var moveY = new Vector2Int(0, moveVector.y);
            
            return IsCellBlocking(origin + moveX) || IsCellBlocking(origin + moveY);
        }

        public Vector2Int GetGridPositionFromWorldPosition(Vector3 position)
        {
            return grid.WorldToCell(position).XY();
        }

        public Vector3 GetWorldPositionFromGridPosition(Vector2Int gridPosition)
        {
            var world = grid.CellToWorld(gridPosition.WithZ(0));
            world = new Vector3(world.x + 0.5f, world.y + 0.5f, 0);
            return world;
        }
        
        private bool TryGetTilemapContainingCell(Vector3Int cellPosition, out Tilemap tilemap)
        {
            foreach (var map in _tilemaps)
            {
                if (!map.cellBounds.Contains(cellPosition)) continue;
                tilemap = map;
                return true;
            }
            tilemap = null;
            return false;
        }

        private bool TryGetTilemapContainingCell(Vector2Int gridPosition, out Tilemap tilemap)
        {
            return TryGetTilemapContainingCell(gridPosition.WithZ(0), out tilemap);
        }

        public static Vector2Int[] GetNeighbourPositions(Vector2Int gridPosition)
        {
            return new[]
            {
                gridPosition+Vector2Int.up,
                gridPosition+Vector2Int.right,
                gridPosition+Vector2Int.down,
                gridPosition+Vector2Int.left,
                gridPosition+new Vector2Int(1, 1),
                gridPosition+new Vector2Int(1, -1),
                gridPosition+new Vector2Int(-1, -1),
                gridPosition+new Vector2Int(-1, 1)
            };
        }

        private List<Node> GetNeighborNodes(Vector2Int gridPosition,Vector2Int startPosition, Vector2Int targetPosition)
        {
            var nodePosition = GetNeighbourPositions(gridPosition);
            var neighborNodes = new List<Node>();

            for (int i = 0; i < 4; i++)
            {
                if (!IsCellBlocking(nodePosition[i])&& !GameManager.Instance.CellContainsEnemy(nodePosition[i]))
                {
                    neighborNodes.Add(new Node(nodePosition[i],startPosition,targetPosition));
                }
            }

            for (int j = 4; j < 8; j++)
            {
                if (!IsCellBlocking(nodePosition[j]) && !IsCellBlockedDiagonal(nodePosition[j],gridPosition) && !GameManager.Instance.CellContainsEnemy(nodePosition[j]))
                {
                    neighborNodes.Add(new Node(nodePosition[j],startPosition,targetPosition));
                } 
            }
            return neighborNodes;
        }

        public Vector2Int GetFreeCell()
        {
            var tilemap = _tilemaps[Random.Range(0, _tilemaps.Count)];
            var bounds = tilemap.cellBounds;
            var maxIterations = 100;
            while (maxIterations > 0)
            {
                var randomPosition = new Vector2Int(Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y));
                if (!IsCellBlocking(randomPosition))
                {
                    return randomPosition;
                }
                maxIterations--;
            }
            return Vector2Int.zero;
        }
        
        public List<Vector2Int> FindShortestDistance(Vector2Int startPosition, Vector2Int targetPosition)
        {
            
            var toSearch = new List<Node>() {new Node(startPosition,startPosition,targetPosition)};
            var processed = new List<Vector2Int>();

            while (toSearch.Count>0)
            {
                var current = toSearch[0];
                foreach (var t in toSearch)
                {
                    if (t.F < current.F || Mathf.Approximately(t.F, current.F) && t.H < current.H)
                    {
                        current = t;
                    }
                }

                processed.Add(current.Position);
                toSearch.Remove(current);

                if (current.Position == targetPosition)
                {
                    var currentPathNode = current;
                    var path = new List<Vector2Int>();
                    while (currentPathNode.Position != startPosition)
                    {
                        path.Add(currentPathNode.Position);
                        currentPathNode = currentPathNode.Connection[0];
                    }

                    return path;
                }

                current.Neighbors = GetNeighborNodes(current.Position,startPosition,targetPosition);
                
                foreach (var neighbor in current.Neighbors.Where(t => !IsCellBlocking(t.Position)))
                {
                    if (processed.Contains(neighbor.Position)) continue;
                    var inSearch = toSearch.Contains(neighbor);
                    var costToNeighbor = current.G + 1;
                    
                    if(!inSearch || costToNeighbor < neighbor.G)
                    {
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetH(neighbor.GetDistance(targetPosition));
                        neighbor.SetConnection(current);
                        
                        if (!inSearch)
                        {
                            toSearch.Add(neighbor);
                        }
                    }
                }
            }
            return null;
        }
    }
    
    
}
