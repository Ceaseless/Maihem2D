
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maihem
{
    public class UIManager : MonoBehaviour
    {
        
        public Slider health;
        public Text healthValue;
        
        public Slider stamina;
        public Text staminaValue;

        [SerializeField] private TextMeshProUGUI distanceCounter;
        [SerializeField] private TextMeshProUGUI currentAttack;
        [SerializeField] private TextMeshProUGUI currentConsumable;

        private int _maxHealth;
        private int _currentHealth;
        private int _maxStamina;
        private int _currentStamina;
        private Vector2Int _startingPoint;
        private Vector2Int _currentPoint;
        

        public void SetMaxHealth(int setHealth)
        {
            _maxHealth = setHealth;
        }

        public void SetCurrentHealth(int newHealth)
        {

            _currentHealth = newHealth;

        }
        
        
        
        public void SetMaxStamina(int setStamina)
        {
            
            _maxStamina = setStamina;
        }

        public void SetCurrentStamina(int newStamina)
        {
            _currentStamina = newStamina;
        }

        public void AdjustDistance(Vector2Int newPoint)
        {
            _currentPoint = newPoint;
        }

        public void UpdateStatus()
        {
            health.maxValue = _maxHealth;
            if (_currentHealth <= 0)  _currentHealth = 0;
            if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
            health.value = _currentHealth;
            var healthValueText = _currentHealth + "/" + _maxHealth + " HP";
            healthValue.text = healthValueText.PadLeft(8,' ');
            
            
            stamina.maxValue = _maxStamina;
            if (_currentStamina <= 0)  _currentStamina = 0;
            if (_currentStamina > _maxStamina) _currentStamina = _maxStamina;
            stamina.value = _currentStamina;
            var staminaValueText = _currentStamina + "/" + _maxStamina;
            staminaValue.text = staminaValueText.PadLeft(8,' ');

            var distance = Math.Abs(Math.Abs(_startingPoint.x - _currentPoint.x) + Math.Abs(_startingPoint.y - _currentPoint.y)/10);
            var distanceCounterText = "Distance: "+ distance +"m";
            distanceCounter.text = distanceCounterText.PadRight(16, ' ');
        }

        public void SetupPlayer(int playerCurrentHealth, int playerCurrentStamina, Vector2Int playerGridPosition)
        {
            _maxHealth = playerCurrentHealth;
            _currentHealth = playerCurrentHealth;

            _maxStamina = playerCurrentStamina;
            _currentStamina = playerCurrentStamina;

            _startingPoint = playerGridPosition;
            _currentPoint = playerGridPosition;

            ChangeAttack("Kick");
            ChangeConsumable("Empty");
            
            UpdateStatus();
        }
        public void UpdatePlayer(int playerCurrentHealth, int playerCurrentStamina, Vector2Int playerGridPosition)
        {
            _currentHealth = playerCurrentHealth;
            
            _currentStamina = playerCurrentStamina;
            
            _currentPoint = playerGridPosition;
            
            UpdateStatus();
        }

        public void ChangeAttack(String attackName)
        {
            currentAttack.text = attackName;
        }
        public void ChangeConsumable(String consumableName)
        {
            currentAttack.text = consumableName;
        }
    }
}
