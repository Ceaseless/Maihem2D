using Maihem.Attacks;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Pickups
{
    public class AttackStrategyPickup : Pickup
    {
        [SerializeField] private AttackStrategy attackStrategy;
        protected override void OnPickUp()
        {
            GameManager.Instance.Player.AddAttackStrategy(attackStrategy);
            PlayOnPickUpEffects();
            IsUsed = true;
        }
    }
}