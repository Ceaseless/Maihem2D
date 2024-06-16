using System.Collections;
using System.Collections.Generic;
using Maihem.Actors;
using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    public class RestorePickup : Pickup
    {
        [SerializeField] private int staminaRestored;
        [SerializeField] private int healthRestored;

        public override void PickUp()
        {
            var player = GameManager.Instance.Player;
            player.AdjustHealthAndStamina(healthRestored,staminaRestored);
        }
    }
}
