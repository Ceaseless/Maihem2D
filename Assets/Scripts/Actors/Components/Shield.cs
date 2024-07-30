using Maihem.Effects;
using Maihem.Managers;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem.Actors.Components
{
    public class Shield : MonoBehaviour
    {
        private static readonly int AnimatorDamageLevel = Animator.StringToHash("Damage Level");
        private static readonly int Activate = Animator.StringToHash("Activate");
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private VisualEffectSettings shieldDestroyEffect;
        [SerializeField] private AudioClip shieldDestroySfx;

        public int CurrentShield { get; private set; }
        public int MaxShield { get; private set; }
        public int LifeTime { get; private set; }
        public int TurnsActive { get; private set; }


        public bool IsActive => TurnsActive <= LifeTime && CurrentShield > 0;

        private void ResetState()
        {
            CurrentShield = 0;
            MaxShield = 0;
            LifeTime = 0;
            TurnsActive = 0;

            animator.SetInteger(AnimatorDamageLevel, 0);
        }

        public void Tick()
        {
            if (LifeTime == 0) return;
            TurnsActive++;
            var color = spriteRenderer.color;
            if (TurnsActive > LifeTime)
            {
                VisualEffectsPool.Instance.PlayFloatingTextEffect("-Shield", Color.cyan, transform.position, false);
                color.a = 0;
                ResetState();
            }
            else
            {
                color.a = 1f - (float)TurnsActive / LifeTime;
            }

            spriteRenderer.color = color;
        }

        public void ActivateShield(int strength, int lifetime)
        {
            spriteRenderer.color = Color.white;
            animator.SetTrigger(Activate);
            LifeTime = lifetime + 1;
            TurnsActive = 0;
            CurrentShield = strength;
            MaxShield = strength;
        }

        public void ReduceShield(int amount)
        {
            CurrentShield = math.max(0, CurrentShield - amount);
            var damageRatio = (float)CurrentShield / MaxShield;

            if (damageRatio <= 0.75 && damageRatio > 0.5) animator.SetInteger(AnimatorDamageLevel, 1);
            if (damageRatio <= 0.5 && damageRatio > 0.25) animator.SetInteger(AnimatorDamageLevel, 2);
            if (damageRatio <= 0.25 && damageRatio > 0) animator.SetInteger(AnimatorDamageLevel, 3);

            var color = spriteRenderer.color;
            if (CurrentShield <= 0)
            {
                VisualEffectsPool.Instance.PlayVisualEffect(shieldDestroyEffect, transform.position);
                AudioManager.Instance.PlaySoundFX(shieldDestroySfx, transform.position, 1f);
                color.a = 0;
                ResetState();
            }

            spriteRenderer.color = color;
        }
    }
}