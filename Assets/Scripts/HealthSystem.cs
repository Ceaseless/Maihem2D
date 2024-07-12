using System;
using System.Collections;
using Maihem.Effects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Maihem
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private GameObject shieldObject;
        [SerializeField] private VisualEffectSettings shieldDestroyEffect;
        [Header("Flash Effect Settings")]
        [SerializeField] private SpriteRenderer parentSpriteRenderer;
        [SerializeField] private float damageFlashDuration = 0.1f;
        [SerializeField] private float healFlashDuration = 0.2f;
        [SerializeField][ColorUsage(true,true)] private Color damageFlashColor = Color.red;
        [SerializeField][ColorUsage(true,true)] private Color healFlashColor = Color.green;
        private SpriteRenderer _shieldRenderer;
        private Animator _shieldAnimator;

        private Material _parentMaterial;
        private bool _isFlashing;
        private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");
        private static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");
        private bool HasShield => _currentShield > 0;
        
        private int _currentShield;
        private int _shieldLifetime;
        private int _turnsActiveShield;
        private int _maxShield;
        private static readonly int AnimatorActivate = Animator.StringToHash("Activate");
        private static readonly int Property = Animator.StringToHash("Damage Level");

        public event EventHandler<HealthChangeEvent> OnHealthChange;
        public bool IsDead => maxHealth == 0 || CurrentHealth <= 0;
        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;

        private void Awake()
        {
            if(parentSpriteRenderer)
                _parentMaterial = parentSpriteRenderer.material;
            
            if (!shieldObject) return;
            _shieldRenderer = shieldObject.GetComponent<SpriteRenderer>();
            _shieldAnimator = shieldObject.GetComponent<Animator>();
        }

        public void Tick()
        {
            if(HasShield) DecayShield();
        }
        
        public void RecoverFullHealth()
        {
            var old = CurrentHealth;
            CurrentHealth = maxHealth;
            OnHealthChange?.Invoke(this, new HealthChangeEvent{ ChangeAmount = CurrentHealth - old });
        }

        public void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0) return;
            if (HasShield)
            {
                ReduceShield(1);
                return;
            }
            var old = CurrentHealth;
            CurrentHealth = math.max(0, CurrentHealth - amount);
            if (!_isFlashing)
            {
                _parentMaterial.SetColor(FlashColorID, damageFlashColor);
                StartCoroutine(PerformFlash(damageFlashDuration));
            }
            OnHealthChange?.Invoke(this, new HealthChangeEvent{ ChangeAmount = old - CurrentHealth });
        }

        public void RecoverHealth(int amount)
        {
            if (amount <= 0) return;
            var old = CurrentHealth;
            CurrentHealth = math.min(maxHealth, CurrentHealth + amount);
            if (!_isFlashing)
            {
                _parentMaterial.SetColor(FlashColorID, healFlashColor);
                StartCoroutine(PerformFlash(healFlashDuration));
            }
            OnHealthChange?.Invoke(this, new HealthChangeEvent{ ChangeAmount = CurrentHealth - old });
        }

        public void AddShield(int strength, int lifetime)
        {
            _shieldRenderer.color = Color.white;
            if (_shieldAnimator && !HasShield)
            {
                _shieldAnimator.SetTrigger(AnimatorActivate);
            }

            _shieldLifetime = lifetime + 1;
            _turnsActiveShield = 0;
            _currentShield = strength+1;
            _maxShield = strength;
            
        }

        private void ReduceShield(int amount)
        {
            _currentShield = math.max(0,_currentShield-amount);
            var damageRatio = (float)_currentShield / _maxShield;
            
            if (damageRatio <= 0.75 && damageRatio > 0.5) _shieldAnimator.SetInteger(Property,1);
            if (damageRatio <= 0.5 && damageRatio > 0.25) _shieldAnimator.SetInteger(Property,2);
            if (damageRatio <= 0.25 && damageRatio > 0) _shieldAnimator.SetInteger(Property,3);
            
            var color = _shieldRenderer.color;
            if (_currentShield <= 0)
            {
                VisualEffectsPool.Instance.PlayVisualEffect(shieldDestroyEffect, transform.position);
                color.a = 0;
                ResetShieldState();
            }
            _shieldRenderer.color = color;
        }

        private void ResetShieldState()
        {
            _currentShield = 0;
            _maxShield = 0;
            _shieldLifetime = 0;
            _turnsActiveShield = 0;
                
            _shieldAnimator.SetInteger(Property,0);
        }

        private void DecayShield()
        {
            _turnsActiveShield++;
            var color = _shieldRenderer.color;
            if (_turnsActiveShield > _shieldLifetime)
            {
                color.a = 0;
                ResetShieldState();
            }
            else
            {
                color.a = 1f - (float) _turnsActiveShield / _shieldLifetime;
            }
            _shieldRenderer.color = color;
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