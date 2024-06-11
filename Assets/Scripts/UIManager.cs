
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Maihem
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        public Slider health;
        public Text healthValue;
        
        public Slider stamina;
        public Text staminaValue;

        public Text killCount;

        private int _maxHealth;
        private int _currentHealth;
        private int _maxStamina;
        private int _currentStamina;

        private int _killCount;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetMaxHealth(int setHealth)
        {
            
            _maxHealth = setHealth;
            _currentHealth = setHealth;
            UpdateStatus();
        }

        public void SetCurrentHealth(int newHealth)
        {
            if (newHealth <= 0)
            {
                _currentHealth = 0;
            }
            else
            {
                _currentHealth = newHealth;
            }
            UpdateStatus();
        }

        public void AdjustHealth(int increase)
        {
            _currentHealth += increase;
            UpdateStatus();
        }
        
        
        
        public void SetMaxStamina(int setStamina)
        {
            
            _maxStamina = setStamina;
            _currentStamina = setStamina;
            UpdateStatus();
        }

        public void SetCurrentStamina(int newStamina)
        {
            if (newStamina <= 0)
            {
                _currentStamina = 0;
            }
            else
            {
                _currentStamina = newStamina;
            }
            UpdateStatus();
        }
        
        public void AdjustStamina(int increase)
        {
            _currentStamina += increase;
            UpdateStatus();
        }

        public void AdjustKillCount(int amount)
        {
            _killCount += amount;
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            health.maxValue = _maxHealth;
            if (_currentHealth <= 0)  _currentHealth = 0;
            if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
            health.value = _currentHealth;
            healthValue.text = _currentHealth + "/" + _maxHealth;
            
            
            stamina.maxValue = _maxStamina;
            if (_currentStamina <= 0)  _currentStamina = 0;
            if (_currentStamina > _maxStamina) _currentStamina = _maxStamina;
            stamina.value = _currentStamina;
            staminaValue.text = _currentStamina + "/" + _maxStamina;

            killCount.text = _killCount.ToString();
        }
    }
}
