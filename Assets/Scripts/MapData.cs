using System.Collections.Generic;
using Maihem.Actors;
using Maihem.Pickups;
using UnityEngine;

namespace Maihem
{
    public class MapData
    {
        public Enemy[] MapEnemies { get; private set; }
        public Pickup[] MapPickups { get; private set; }

        public MapData(Enemy[] enemies, Pickup[] pickups)
        {
            MapEnemies = enemies;
            MapPickups = pickups;
        }
        
            
    }
}