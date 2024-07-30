using System;
using UnityEngine;

namespace Maihem.Maps
{
    [Serializable]
    public class SpawnSlot
    {
        public GameObject[] includeSpawns;
        public GameObject[] excludeSpawns;
    }
}