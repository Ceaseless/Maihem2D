using Maihem.Managers;
using UnityEngine;

namespace Maihem.Pickups
{
    public class ConsumablePickup : Pickup
    {
        [SerializeField] private Consumable consumable;


        new void Start()
        {
            SnapToGrid();
            spriteRenderer.color = pickupColor;
            consumable.sprite = spriteRenderer.sprite;
        }
        protected override void OnPickUp()
        {
            var player = GameManager.Instance.Player;
            if (player.consumable) return;
            player.consumable = consumable;
        }
    }
}
