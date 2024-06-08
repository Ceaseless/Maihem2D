
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

        public int G { get; private set; }
        public int H { get; private set; }
        public int F { get; private set; }

        public Node[] Connection;

        public Node(Vector2Int position)
        {
            Position = position;
            G = int.MaxValue;
            H = int.MaxValue;
            F = G + H;
            Connection = null;
        }

        public void SetG(int g) => G = g;

        public void SetH(int h) => H = h;
        
        public void CalculateF() => F = G + H;

        public void SetConnection(Node connect) => Connection = new[] { connect };

        public int GetDistance(Vector2Int target)
        {
            var xDistance = Mathf.Abs(Position.x - target.x);
            var yDistance = Mathf.Abs(Position.y - target.y);
            var remaining = Mathf.Abs((xDistance - yDistance));
            return Mathf.Min(xDistance, yDistance) + remaining; 
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

        private bool IsCellBlocking(Vector2Int gridPosition)
        {
            var cellPosition = gridPosition.WithZ(0);
            
            if (!TryGetTilemapContainingCell(gridPosition, out var map)) return true;
            
            var cell = map.GetTile<Tile>(cellPosition);
            return cell is null || cell.colliderType != Tile.ColliderType.None;
        }

        private bool IsCellBlockedDiagonal(Vector2Int gridPosition, Vector2Int origin)
        {
            var moveVector = new Vector2Int(gridPosition.x - origin.x,gridPosition.y-origin.y);
            var moveX = new Vector2Int(moveVector.x, 0);
            var moveY = new Vector2Int(0, moveVector.y);
            
            var diagonalBlock = IsCellBlocking(origin + moveX) || IsCellBlocking(origin + moveY);
            return diagonalBlock;
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

        private List<Node> GetNeighborNodes(Vector2Int gridPosition)
        {
            var nodePosition = GetNeighbourPositions(gridPosition);
            var neighborNodes = new List<Node>();
            
            for (int i = 0; i < 4; i++)
            {
                if (!IsCellBlocking(nodePosition[i])&& !GameManager.Instance.CellContainsEnemy(nodePosition[i]))
                {
                    neighborNodes.Add(new Node(nodePosition[i]));
                }
            }

            for (int j = 4; j < 8; j++)
            { 
                if (!IsCellBlocking(nodePosition[j])  && !GameManager.Instance.CellContainsEnemy(nodePosition[j]))
                {
                   if (!IsCellBlockedDiagonal(nodePosition[j], gridPosition))
                   {
                        neighborNodes.Add(new Node(nodePosition[j])); 
                   }
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
            
            var startNode = new Node(startPosition);
            
            startNode.SetG(0);
            startNode.SetH(startNode.GetDistance(targetPosition));
            startNode.CalculateF();
            
            var toSearch = new List<Node>() {startNode};
            var processed = new List<Vector2Int>();
            
            while (toSearch.Count > 0)
            {
                var current = toSearch.First();
                foreach (var t in toSearch)
                {
                    if (t.F < current.F || (t.F == current.F && t.H < current.H))
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
                
                
                
                foreach (var neighbor in GetNeighborNodes(current.Position))
                {
                    if (processed.Contains(neighbor.Position)) continue;
                 
                    var costToNeighbor = current.G + current.GetDistance(neighbor.Position);
                    
                    if(!IsNodeInList(neighbor,toSearch) || costToNeighbor < neighbor.G)
                    {
                        neighbor.SetConnection(current);
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetH(neighbor.GetDistance(targetPosition));
                        neighbor.CalculateF();
                        
                        if (!IsNodeInList(neighbor,toSearch))
                        {
                            toSearch.Add(neighbor);
                        }
                        
                    }
                }
            }
            return null;
        }

        private bool IsNodeInList(Node t, List<Node> list)
        {
            foreach (var node in list)
            {
                if (node.Position == t.Position) return true;
            }

            return false;
        }
    }
    
    
}
