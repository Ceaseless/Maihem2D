using Maihem.Actors;
using Maihem.Pickups;

namespace Maihem.Maps
{
    public class MapData
    {
        public MapData(Enemy[] enemies, Pickup[] pickups)
        {
            MapEnemies = enemies;
            MapPickups = pickups;
        }

        public Enemy[] MapEnemies { get; private set; }
        public Pickup[] MapPickups { get; private set; }
    }
}