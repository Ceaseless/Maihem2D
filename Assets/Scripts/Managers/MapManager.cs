using System.Collections.Generic;
using System.Linq;
using Maihem.Actors;
using Maihem.Extensions;
using Maihem.Pickups;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Maihem.Managers
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
        [SerializeField] private PolygonCollider2D mapConstraints;
        [SerializeField] private GameObject mapPrefab;
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
            _tilemaps = new List<Tilemap>();
            SpawnMap();
        }

        public void Reset()
        {
            foreach (var map in _tilemaps)
            {
                Destroy(map.gameObject);
            }
            _tilemaps.Clear();
            SpawnMap();
        }

        private void SpawnMap()
        {
            var mapObject = Instantiate(mapPrefab, grid.transform);
            var tileMap = mapObject.GetComponent<Tilemap>();

            var enemies = mapObject.GetComponentsInChildren<Enemy>();
            var pickups = mapObject.GetComponentsInChildren<Pickup>();
            
            tileMap.CompressBounds();
            _tilemaps.Add(tileMap);
            GameManager.Instance.PassMapData(new MapData(enemies, pickups));
        }
        
        public void UpdateMap()
        {
            // TODO
        }

        public bool IsCellBlocking(Vector3 worldPosition)
        {
            var cellPosition = WorldToCell(worldPosition);
            return IsCellBlocking(cellPosition);
        }

        public bool IsCellBlocking(Vector2Int cellPosition)
        {
            var position = cellPosition.WithZ(0);
            if (!TryGetTile(position, out var tile)) return true;
            return tile is null || tile.colliderType != Tile.ColliderType.None;
            //if (!TryGetTilemapContainingCell(position, out var map)) return true;

            //var cell = map.GetTile<Tile>(position);
            //return cell is null || cell.colliderType != Tile.ColliderType.None;
        }

        private bool TryGetTile(Vector3Int cellPosition, out Tile tile)
        {
            foreach (var map in _tilemaps)
            {
                var localPosition = new Vector3Int((int)(cellPosition.x - map.transform.localPosition.x),
                    (int)(cellPosition.y - map.transform.localPosition.y), 0);
                
                if (!map.HasTile(localPosition)) continue;
                tile = map.GetTile<Tile>(localPosition);
                return true;
            }
            tile = null;
            return false;
        }

        public bool IsCellBlockedDiagonal(Vector2Int cellPosition, Vector2Int origin)
        {
            var moveVector = new Vector2Int(cellPosition.x - origin.x,cellPosition.y-origin.y);
            var moveX = new Vector2Int(moveVector.x, 0);
            var moveY = new Vector2Int(0, moveVector.y);
            
            var diagonalBlock = IsCellBlocking(origin + moveX) || IsCellBlocking(origin + moveY);
            return diagonalBlock;
        }

        public Vector2Int WorldToCell(Vector3 position)
        {
            return grid.WorldToCell(position).XY();
        }

        public Vector3 CellToWorld(Vector2Int cellPosition)
        {
            var world = grid.CellToWorld(cellPosition.WithZ(0));
            world = new Vector3(world.x + 0.5f, world.y + 0.5f, 0);
            return world;
        }
        
        private bool TryGetTilemapContainingCell(Vector3Int cellPosition, out Tilemap tilemap)
        {
            foreach (var map in _tilemaps)
            {
                var localPosition = new Vector3Int((int)(cellPosition.x - map.transform.localPosition.x),
                    (int)(cellPosition.y - map.transform.localPosition.y), 0);
                if (!map.HasTile(localPosition)) continue;
                tilemap = map;
                return true;
            }
            tilemap = null;
            return false;
        }

        private bool TryGetTilemapContainingCell(Vector2Int cellPosition, out Tilemap tilemap)
        {
            return TryGetTilemapContainingCell(cellPosition.WithZ(0), out tilemap);
        }

        public static IList<Vector2Int> GetNeighbourPositions(Vector2Int cellPosition)
        {
            return new List<Vector2Int>
            {
                cellPosition+Vector2Int.up,
                cellPosition+Vector2Int.right,
                cellPosition+Vector2Int.down,
                cellPosition+Vector2Int.left,
                cellPosition+new Vector2Int(1, 1),
                cellPosition+new Vector2Int(1, -1),
                cellPosition+new Vector2Int(-1, -1),
                cellPosition+new Vector2Int(-1, 1)
            };
        }

        private IList<Vector2Int> GetFreeNeighbours(Vector2Int cellPosition)
        {
            var freeNeighbours = GetNeighbourPositions(cellPosition);

            foreach (var neighbour in freeNeighbours.ToList())
            {
                if (IsCellBlocking(neighbour) || IsCellBlockedDiagonal(neighbour, cellPosition))
                {
                    freeNeighbours.Remove(neighbour);
                }
            }
            
            return freeNeighbours;
        }
        
        public List<Vector2Int> IsInDirectLine(Vector2Int cellPosition, Vector2Int target, int range)
        {
            int diffX = cellPosition.x - target.x;
            int diffY = cellPosition.y - target.y;

            List<Vector2Int> targetLine = new List<Vector2Int>();
            Vector2Int checkDirection = cellPosition;
            
            int directionX = 0;
            int directionY = 0;

            if (diffX > 0) directionX = -1;
            if (diffX < 0) directionX = 1;
            if (diffY > 0) directionY = -1;
            if (diffY < 0) directionY = 1;

            for (int i = 0; i < range; i++)
            {
                checkDirection = new Vector2Int(checkDirection.x + directionX, checkDirection.y + directionY);
                if (!IsCellBlocking(checkDirection) && !IsCellBlockedDiagonal(checkDirection, cellPosition))
                {
                   targetLine.Add(checkDirection); 
                }
            }

            return targetLine;
        }

        private List<Node> GetNeighborNodes(Vector2Int cellPosition)
        {
            var nodePosition = GetNeighbourPositions(cellPosition);
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
                   if (!IsCellBlockedDiagonal(nodePosition[j], cellPosition))
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
            var path = new List<Vector2Int>();
            
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


            var freeNeighbours = GetFreeNeighbours(startPosition);
            if (freeNeighbours.Count <= 0) 
            { 
                path.Add(startPosition); 
                return path;
            }

            freeNeighbours = freeNeighbours.OrderBy(x => x.ManhattanDistance(targetPosition)).ToList();
            path.Add(freeNeighbours[0]);
     
            return path;
        }

        private static bool IsNodeInList(Node t, List<Node> list)
        {
            return list.Any(node => node.Position == t.Position);
        }
    }
    
    
}
