using UnityEngine;

namespace Maihem.Maps
{
    [CreateAssetMenu(fileName = "Map Spawn Order")]
    public class MapSpawnOrder : ScriptableObject
    {
        public SpawnSlot[] spawnSlots;
    }
}