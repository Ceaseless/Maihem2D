using Maihem.Effects;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Pickups
{
    public class RestorePickup : Pickup
    {
        [SerializeField] private int staminaRestored;
        [SerializeField] private int healthRestored;

        protected override void OnPickUp()
        {
            var player = GameManager.Instance.Player;
            player.healthSystem.RecoverHealth(healthRestored);
            player.RecoverStamina(staminaRestored);
            VisualEffectsPool.Instance.PlayVisualEffect(VisualEffectType.Recover, transform.position);
        }
    }
}
