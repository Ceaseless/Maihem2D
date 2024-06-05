using System.Collections.Generic;
using System.Linq;
using Maihem.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Maihem
{
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

        

    }
}
