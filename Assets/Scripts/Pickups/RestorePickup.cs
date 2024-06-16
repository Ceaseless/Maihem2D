using Maihem.Managers;
using UnityEngine;

namespace Maihem.Pickups
{
    public class RestorePickup : Pickup
    {
        [SerializeField] private int staminaRestored;
        [SerializeField] private int healthRestored;

        protected override void PickUp()
        {
            GameManager.Instance.Player.AdjustHealthAndStamina(healthRestored,staminaRestored);
        }
    }
}
