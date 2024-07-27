using UnityEngine;

namespace Maihem.Effects
{
    // For one shot animation based effects
    // DisableGameObject should be called from an animation event
    [RequireComponent(typeof(Animator))]
    public class OneShotAnimationEffect : MonoBehaviour
    {
        public void DisableGameObject()
        {
            gameObject.SetActive(false);
        }
    }
}