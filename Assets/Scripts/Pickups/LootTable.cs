using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maihem.Pickups;
using UnityEngine;
using static UnityEngine.Random;


namespace Maihem
{
    [CreateAssetMenu(menuName = "Loot Tabel")]
    public class LootTable : ScriptableObject
    {
        
        [SerializeField] private List<Loot> possibleItems;

        public GameObject rollOnLootTable()
        {
            var totalWeight = possibleItems.Sum(item => item.dropChance);
            var currentWeight = 0;
            var roll = Range(0, totalWeight);
            
            for(var i = 0; i<possibleItems.Count;i++)
            {
                currentWeight += possibleItems[i].dropChance;
                if (roll < currentWeight)
                {
                    return possibleItems[i].droppedLoot;
                }

            }

            return null;
        }

    }

    [Serializable]
    public struct Loot
    {
        public GameObject droppedLoot;
        public int dropChance;
    }
}
