using System;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private GameObject shieldObject;

        private SpriteRenderer _shieldRenderer;
        private Animator _shieldAnimator;

        private bool HasShield => _currentShield > 0;
        
        private int _currentShield;
        private int _maxShield;
        private static readonly int AnimatorActivate = Animator.StringToHash("Activate");

        public event EventHandler<HealthChangeEvent> OnHealthChange;
        public bool IsDead => maxHealth == 0 || CurrentHealth <= 0;
        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;

        private void Awake()
        {
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
            OnHealthChange?.Invoke(this, new HealthChangeEvent{ ChangeAmount = old - CurrentHealth });
        }

        public void RecoverHealth(int amount)
        {
            if (amount <= 0) return;
            var old = CurrentHealth;
            CurrentHealth = math.min(maxHealth, CurrentHealth + amount);
            OnHealthChange?.Invoke(this, new HealthChangeEvent{ ChangeAmount = CurrentHealth - old });
        }

        public void AddShield(int strength)
        {
            _shieldRenderer.color = Color.white;
            if (_shieldAnimator && !HasShield)
            {
                _shieldAnimator.SetTrigger(AnimatorActivate);
            }
            
            _currentShield = strength+1;
            _maxShield = strength;
            
        }

        private void ReduceShield(int amount)
        {
            _currentShield = math.max(0,_currentShield-amount);
            var color = _shieldRenderer.color;

            if (_currentShield <= 0)
            {
                color.a = 0;
            }
            else
            {
                color.g = (float)_currentShield/_maxShield;
                color.b = (float)_currentShield/_maxShield; 
            }
            _shieldRenderer.color = color;
        }

        private void DecayShield()
        {
            ReduceShield(1);
        }
    }
    
    public class HealthChangeEvent : EventArgs
    {
        public int ChangeAmount { get; set; }
    }
}