using Maihem.Actors;
using Maihem.Pickups;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Maihem.Maps
{
   
    public class MapChunk : MonoBehaviour
    {
        [field: SerializeField] public Tilemap GroundLayer { get; private set; }
        [field: SerializeField] public Tilemap CollisionLayer { get; private set; }
        [field: SerializeField] public Transform EnemyParent { get; private set; }
        [field: SerializeField] public Transform PickupParent { get; private set; }
        [field: SerializeField] public Transform PotentialStartPosition { get; private set; }
        [field: SerializeField] public Transform PotentialGoalPosition { get; private set; }

        public MapData GetMapData()
        {
            var enemies = EnemyParent.GetComponentsInChildren<Enemy>();
            var pickups = PickupParent.GetComponentsInChildren<Pickup>();
            return new MapData(enemies, pickups);
        }
    }
}