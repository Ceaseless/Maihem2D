using Maihem.Effects;
using Maihem.Enums;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Pickups
{
    [CreateAssetMenu(menuName = "Consumable")]
    public class Consumable : ScriptableObject
    {
        [SerializeField] public ConsumableType type;
        [SerializeField] public Sprite sprite;
        [SerializeField] public VisualEffectSettings activateVisualEffect;
        [SerializeField] public AudioClip activateSoundEffect;

        public void PlayActivateEffects(Vector3 position)
        {
            if (activateVisualEffect is not null)
            {
                VisualEffectsPool.Instance.PlayVisualEffect(activateVisualEffect, position);
            }

            if (activateSoundEffect is not null)
            {
                AudioManager.Instance.PlaySoundFX(activateSoundEffect, position, 1f);
            }
        }
    }
}
