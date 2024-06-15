using Maihem.Actors;
using UnityEngine;

namespace Maihem
{
    public class MapData : MonoBehaviour
    {
        [SerializeField] private Enemy[] presentEnemies;
        [SerializeField] private Pickup[] presentPickups;

        public Enemy[] MapEnemies => presentEnemies;
        public Pickup[] MapPickups => presentPickups;

    }
}