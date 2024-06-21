using System;
using Unity.Mathematics;
using UnityEngine;

namespace Maihem
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private int maxHealth;

        public EventHandler<HealthChangeEvent> OnHealthChange;
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
            var old = CurrentHealth;
            CurrentHealth = math.max(0, CurrentHealth - amount);
            OnHealthChange?.Invoke(this, new HealthChangeEvent{ ChangeAmount = old - CurrentHealth });
        }

        public void RecoverHealth(int amount)
        {
            if (amount <= 0) return;
            var old = CurrentHealth;
            CurrentHealth = math.max(maxHealth, CurrentHealth + amount);
            OnHealthChange?.Invoke(this, new HealthChangeEvent{ ChangeAmount = CurrentHealth - old });
        }
    }
    
    public class HealthChangeEvent : EventArgs
    {
        public int ChangeAmount { get; set; }
    }
}