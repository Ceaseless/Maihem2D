using Maihem.Enums;
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
            if (player.currentConsumable.type != ConsumableType.Empty)
            {
                GameManager.Instance.ItemButtonFlash("Red");
                return;
            }

            player.currentConsumable = consumable;
            GameManager.Instance.ItemButtonFlash("Green");
            PlayOnPickUpEffects();
            IsUsed = true;
        }
    }
}