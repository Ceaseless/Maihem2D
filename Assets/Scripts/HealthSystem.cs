using System;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private SpriteRenderer shield;

        private float _shield;
        private float _maxShield;

        public event EventHandler<HealthChangeEvent> OnHealthChange;
        public bool IsDead => maxHealth == 0 || CurrentHealth <= 0;
        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;

        public void RecoverFullHealth()
        {
            var old = CurrentHealth;
            CurrentHealth = maxHealth;
            OnHealthChange?.Invoke(this, new HealthChangeEvent{ ChangeAmount = CurrentHealth - old });
        }

        public void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0) return;
            if (_shield > 0)
            {
                ReduceShield();
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
            _shield = strength+1;
            _maxShield = strength;
            var color = new Color(1,1,1,1);
            shield.color = color;
        }

        private void ReduceShield()
        {
            _shield -= 1;
            var color = shield.color;

            if (_shield <= 0)
            {
                color.a = 0;
            }
            else
            {
                color.g = _shield/_maxShield;
                color.b = _shield/_maxShield; 
            }
            shield.color = color;
        }

        public void DecayShield()
        {
            if (_shield > 0)
            {
                ReduceShield();
            }
        }
    }
    
    public class HealthChangeEvent : EventArgs
    {
        public int ChangeAmount { get; set; }
    }
}