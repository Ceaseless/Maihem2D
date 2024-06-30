using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maihem
{
    [CreateAssetMenu(menuName = "Consumable")]
    public class Consumable : ScriptableObject
    {
        [SerializeField] public ConsumableType type;
        [SerializeField] public Sprite sprite;
    }
}
