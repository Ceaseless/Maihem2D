using Maihem.Effects;
using UnityEngine;

namespace Maihem.Pickups
{
    [CreateAssetMenu(menuName = "Consumable")]
    public class Consumable : ScriptableObject
    {
        [SerializeField] public ConsumableType type;
        [SerializeField] public Sprite sprite;
        [SerializeField] public VisualEffectSettings visualEffect;
    }
}
