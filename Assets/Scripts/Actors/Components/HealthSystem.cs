using System;
using System.Collections;
using Maihem.Effects;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem.Actors.Components
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private Shield shield;
        [Header("Flash Effect Settings")]
        [SerializeField] private SpriteRenderer parentSpriteRenderer;
        [SerializeField] private float damageFlashDuration = 0.1f;
        [SerializeField] private float healFlashDuration = 0.2f;
        [SerializeField][ColorUsage(true,true)] private Color damageFlashColor = Color.red;
        [SerializeField] [ColorUsage(true, true)] private Color healFlashColor = Color.green;
        [SerializeField][ColorUsage(true,true)] private Color shieldFlashColor = Color.blue;

        private Material _parentMaterial;
        private bool _isFlashing;
        private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");
        private static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");
       
        public event EventHandler<HealthChangeEvent> OnHealthChange;
        public bool IsDead => maxHealth == 0 || CurrentHealth <= 0;
        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;

        private void Awake()
        {
            if(parentSpriteRenderer)
                _parentMaterial = parentSpriteRenderer.material;
        }

        public void Tick()
        {
            shield.Tick();
        }
        
        public void RecoverFullHealth()
        {
            var old = CurrentHealth;
            CurrentHealth = maxHealth;
            OnHealthChange?.Invoke(this, new HealthChangeEvent{ ChangeAmount = CurrentHealth - old });
        }

        public void AddShield(int strength, int lifetime)
        {
            shield.ActivateShield(strength, lifetime);
        }

        public void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0) return;
            if (shield.IsActive)
            {
                Flash(shieldFlashColor, damageFlashDuration);
                shield.ReduceShield(1);
                return;
            }
            var old = CurrentHealth;
            CurrentHealth = math.max(0, CurrentHealth - amount);
            Flash(damageFlashColor, damageFlashDuration);
            
            VisualEffectsPool.Instance.PlayFloatingTextEffect($"-{amount}", damageFlashColor, transform.position);
            OnHealthChange?.Invoke(this, new HealthChangeEvent{ ChangeAmount = old - CurrentHealth });
        }

        public void RecoverHealth(int amount)
        {
            if (amount <= 0) return;
            var old = CurrentHealth;
            CurrentHealth = math.min(maxHealth, CurrentHealth + amount);
            Flash(healFlashColor,healFlashDuration);
            VisualEffectsPool.Instance.PlayFloatingTextEffect($"+{amount}HP", healFlashColor, transform.position);
            OnHealthChange?.Invoke(this, new HealthChangeEvent{ ChangeAmount = CurrentHealth - old });
        }

        public void Flash(Color color, float duration)
        {
            if (_isFlashing) return;
            _parentMaterial.SetColor(FlashColorID, color);
            StartCoroutine(PerformFlash(duration));
        }

        private IEnumerator PerformFlash(float duration)
        {
            _isFlashing = true;
            var time = 0f;
            var halfTime = duration * 0.5f;
            while (time < halfTime)
            {
                _parentMaterial.SetFloat(FlashAmountID, Mathf.Lerp(0, 1, time / halfTime));
                time += Time.deltaTime;
                yield return null;
            }
            _parentMaterial.SetFloat(FlashAmountID, 1f);
            time = 0f;
            while (time < halfTime)
            {
                _parentMaterial.SetFloat(FlashAmountID, Mathf.Lerp(1, 0, time / halfTime));
                time += Time.deltaTime;
                yield return null;
            }
            _parentMaterial.SetFloat(FlashAmountID, 0f);
            _isFlashing = false;
        }
    }
    
    public class HealthChangeEvent : EventArgs
    {
        public int ChangeAmount { get; set; }
    }
}