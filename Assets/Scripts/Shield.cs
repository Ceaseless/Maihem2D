using Maihem.Effects;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem
{
    public class Shield : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private VisualEffectSettings shieldDestroyEffect;
        
        public int CurrentShield { get; private set; }
        public int MaxShield { get; private set; }
        public int LifeTime { get; private set; }
        public int TurnsActive { get; private set; }


        public bool IsActive => TurnsActive <= LifeTime && CurrentShield > 0;

        private static readonly int AnimatorDamageLevel = Animator.StringToHash("Damage Level");
        private static readonly int Activate = Animator.StringToHash("Activate");

        private void ResetState()
        {
            CurrentShield = 0;
            MaxShield = 0;
            LifeTime = 0;
            TurnsActive = 0;
                
            animator.SetInteger(AnimatorDamageLevel,0);
        }
        
        public void Tick()
        {
            TurnsActive++;
            var color = spriteRenderer.color;
            if (TurnsActive > LifeTime)
            {
                color.a = 0;
                ResetState();
            }
            else
            {
                color.a = 1f - (float) TurnsActive / LifeTime;
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
            CurrentShield = math.max(0,CurrentShield-amount);
            var damageRatio = (float)CurrentShield / MaxShield;
            
            if (damageRatio <= 0.75 && damageRatio > 0.5) animator.SetInteger(AnimatorDamageLevel,1);
            if (damageRatio <= 0.5 && damageRatio > 0.25) animator.SetInteger(AnimatorDamageLevel,2);
            if (damageRatio <= 0.25 && damageRatio > 0) animator.SetInteger(AnimatorDamageLevel,3);
            
            var color = spriteRenderer.color;
            if (CurrentShield <= 0)
            {
                VisualEffectsPool.Instance.PlayVisualEffect(shieldDestroyEffect, transform.position);
                color.a = 0;
                ResetState();
            }
            spriteRenderer.color = color;
        }
    }
}
