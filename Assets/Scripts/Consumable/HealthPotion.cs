using System.Collections;
using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    public class HealthPotion : Consumable
    {
        [SerializeField] private int healthRestored = 5;
        protected override void OnActivation()
        {
            var player = GameManager.Instance.Player;
            player.healthSystem.RecoverHealth(healthRestored);
        }
    }
}
