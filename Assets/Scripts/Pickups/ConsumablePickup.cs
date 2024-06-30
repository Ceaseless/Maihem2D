using Maihem.Managers;
using UnityEngine;

namespace Maihem.Pickups
{
    public class ConsumablePickup : Pickup
    {
        [SerializeField] private Consumable consumable;
        protected override void OnPickUp()
        {
            var player = GameManager.Instance.Player;
            if (player.consumable.type != ConsumableType.Empty) return;
            player.consumable = consumable;
            IsUsed = true;
        }
    }
}
