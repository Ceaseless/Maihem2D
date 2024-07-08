using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Maihem.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private CinemachineConfiner2D cameraConfiner;
        [SerializeField] private GameObject goalPrefab;
        [SerializeField] private GameObject[] mapPrefabs;
        [SerializeField] private GameObject tutorialPrefab;
        [SerializeField] private float mapSpawnDistance = 20f;
        [SerializeField] private bool tutorialCompleted;
        private List<MapChunk> _mapChunks;
        private int _instantiatedMapChunks;
        private int _currentMaxX;
        
        
        
        public static readonly Vector2Int[] CellNeighborOffsets =
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left,
            new(1, 1),
            new(1, -1),
            new(-1, -1),
            new(-1, 1)
        };
        
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
            tutorialCompleted = !MenuManager.TutorialActivated;
            _mapChunks = new List<MapChunk>();
            SpawnMap();
        }

        public void Reset()
        {
            foreach (var chunk in _mapChunks)
            {
                Destroy(chunk.gameObject);
            }
            _mapChunks.Clear();
            _instantiatedMapChunks = 0;
            _currentMaxX = 0;
            SpawnMap();
            
        }

        private void SpawnMap(int index = 0)
        {
            GameObject mapObject;
            
            if (!tutorialCompleted && tutorialPrefab)
            {
                mapObject = Instantiate(tutorialPrefab, grid.transform);
            }
            else
            {
                if (index < 0 || index >= mapPrefabs.Length)
                {
                    Debug.LogError("Not enough map prefabs set!");
                    return;
                }
                mapObject = Instantiate(mapPrefabs[index], grid.transform);
                
                if (index > 0)
                {
                    var predecessor = _mapChunks[index - 1];
                    var prePosition = predecessor.transform.localPosition;
                    mapObject.transform.localPosition = new Vector3(_currentMaxX, prePosition.y, 0);
                }
            }
            
            var mapChunk = mapObject.GetComponent<MapChunk>();
            var tileMap = mapChunk.TileMap;
            
            tileMap.CompressBounds();
            var bounds = tileMap.cellBounds;
            var oldBounds = mapConstraints.GetPath(0);
            var path = new[]
            {
                oldBounds[0],
                oldBounds[1],
                new Vector2(bounds.xMax*(_instantiatedMapChunks+1), bounds.yMax),
                new Vector2(bounds.xMax*(_instantiatedMapChunks+1), bounds.yMin)
            };
            
            _currentMaxX = (int)mapObject.transform.localPosition.x + bounds.xMax;
            mapConstraints.SetPath(0,path);
            cameraConfiner.InvalidateCache();
            _mapChunks.Add(mapChunk);
            GameManager.Instance.PassMapData(mapChunk.GetMapData());
            _instantiatedMapChunks++;
            if (_instantiatedMapChunks == mapPrefabs.Length)
            {
                Instantiate(goalPrefab, mapChunk.PotentialGoalPosition.position, Quaternion.identity, mapChunk.transform);
            }
            
            
        }
        
        // Just keep spawning maps if player is too close to the end until we run out of prefabs
        public void UpdateMap()
        {
            if (_instantiatedMapChunks >= mapPrefabs.Length || !tutorialCompleted) return;
            var lastChunk = _mapChunks[_instantiatedMapChunks - 1];
            var playerPosition = GameManager.Instance.Player.transform.position;
            if (Mathf.Abs(playerPosition.x - lastChunk.PotentialGoalPosition.position.x) < mapSpawnDistance)
            {
                SpawnMap(_instantiatedMapChunks);
            }
        }

        public bool IsCellBlocking(Vector3 worldPosition)
        {
            var cellPosition = WorldToCell(worldPosition);
            return IsCellBlocking(cellPosition);
        }

        public bool IsCellBlocking(Vector2Int cellPosition)
        {
            for (var z = 0; z < 2; z++)
            {
                var position = cellPosition.WithZ(z);
                var tile = TryGetTile(position);
                if (z == 0 && tile is null) return true;
                if (tile is not null && tile.colliderType != Tile.ColliderType.None) return true;
            }
            return false;
        }

        private Tile TryGetTile(Vector3Int cellPosition)
        {
            Tile tile = null;
            foreach (var chunk in _mapChunks)
            {
                var localPosition = new Vector3Int((int)(cellPosition.x - chunk.transform.localPosition.x),
                    (int)(cellPosition.y - chunk.transform.localPosition.y), (int)(cellPosition.z - chunk.transform.localPosition.z));
                
                if (!chunk.TileMap.HasTile(localPosition)) continue;
                tile = chunk.TileMap.GetTile<Tile>(localPosition);
            }
            return tile;
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
            foreach (var chunk in _mapChunks)
            {
                var localPosition = new Vector3Int((int)(cellPosition.x - chunk.transform.localPosition.x),
                    (int)(cellPosition.y - chunk.transform.localPosition.y), 0);
                if (!chunk.TileMap.HasTile(localPosition)) continue;
                tilemap = chunk.TileMap;
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
            var neighbours = new List<Vector2Int>(CellNeighborOffsets.Length);
            neighbours.AddRange(CellNeighborOffsets.Select(offset => cellPosition + offset));
            return neighbours;
        }
        
        
        public bool IsInDirectLine(Vector2Int origin, Vector2Int target, int range)
        {
            var dir = new Vector2Int(math.clamp(target.x-origin.x, -1, 1),math.clamp(target.y - origin.y, -1, 1));
            for (var i = 1; i <= range; i++)
            {
                var checkPosition = origin + dir*i;
                if (IsCellBlocking(checkPosition)) return false;
            }
            return true;
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
            var randomChunk = _mapChunks[Random.Range(0, _mapChunks.Count)];
            var bounds = randomChunk.TileMap.cellBounds;
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

            var maxIterations = 0;
            
            while (toSearch.Count > 0)
            {
                if (maxIterations >= 2500)
                {
                    Debug.Log("Exceeded iterations");
                    Debug.Log($"Could not find path from {startPosition} to {targetPosition}");
                    Debug.DrawLine(CellToWorld(startPosition), CellToWorld(targetPosition));
                    return new List<Vector2Int>();
                }
                
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

                maxIterations++;
            }

            var lowestDistance = int.MaxValue;
            var bestNode = -1;
            for(var i = 0; i < CellNeighborOffsets.Length; i++)
            {
                var neighbor = startPosition + CellNeighborOffsets[i];
                if(IsCellBlocking(neighbor) || IsCellBlockedDiagonal(neighbor, startPosition) || GameManager.Instance.CellContainsActor(neighbor)) continue;
                var distance = neighbor.ManhattanDistance(targetPosition);
                if (distance < lowestDistance)
                {
                    lowestDistance = distance;
                    bestNode = i;
                }
            }

            if (bestNode < 0)
            {
                path.Add(startPosition);
                return path;
            }
            
            path.Add(startPosition + CellNeighborOffsets[bestNode]);

            // var freeNeighbours = GetFreeNeighbours(startPosition);
            // if (freeNeighbours.Count <= 0) 
            // { 
            //     path.Add(startPosition); 
            //     return path;
            // }
            //
            // freeNeighbours = freeNeighbours.OrderBy(x => x.ManhattanDistance(targetPosition)).ToList();
            // path.Add(freeNeighbours[0]);
     
            return path;
        }

        private static bool IsNodeInList(Node t, List<Node> list)
        {
            return list.Any(node => node.Position == t.Position);
        }

        public void TutorialFinished()
        {
            tutorialCompleted = true;
        }
    }
    
    
}
