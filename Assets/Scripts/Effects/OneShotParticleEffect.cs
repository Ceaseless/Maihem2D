using UnityEngine;

namespace Maihem.Effects
{
    // For "Play On Awake" based particle effects
    // Have to be set to active from the outside
    public class OneShotParticleEffect : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem effectParticleSystem;

        private void Update()
        {
            if (!effectParticleSystem.isPlaying)
            {
                gameObject.SetActive(false);
            }
        }
    }
}