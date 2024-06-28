using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maihem.Managers
{
    public class UIManager : MonoBehaviour
    {
        
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthValue;
        
        [SerializeField] private Slider staminaBar;
        [SerializeField] private TextMeshProUGUI staminaValue;

        [SerializeField] private TextMeshProUGUI distanceCounter;
        [SerializeField] private TextMeshProUGUI currentAttack;
        [SerializeField] private TextMeshProUGUI currentConsumable;

        [SerializeField] private GameObject winScreen;
        
        private Vector2Int _startingPoint;
        
        public void Initialize()
        {
            _startingPoint = GameManager.Instance.Player.GridPosition;
            GameManager.Instance.Player.OnStatusUpdate += UpdateStatusUI;
            winScreen.SetActive(false);
            UpdateStatusUI(this, EventArgs.Empty);
        }
        
        private void UpdateStatusUI(object sender, EventArgs e)
        {
            var player = GameManager.Instance.Player;
            
            healthBar.maxValue = player.healthSystem.MaxHealth;
            healthBar.value = player.healthSystem.CurrentHealth;
            var healthValueText = player.healthSystem.CurrentHealth + "/" + player.healthSystem.MaxHealth;
            healthValue.text = healthValueText;
            
            staminaBar.maxValue = player.MaxStamina;
            staminaBar.value = player.CurrentStamina;
            var staminaValueText = player.CurrentStamina + "/" + player.MaxStamina;
            staminaValue.text = staminaValueText;

            //var distance = Math.Abs(_startingPoint.x - player.GridPosition.x);
            var distance = Math.Abs(Math.Abs(_startingPoint.x - player.GridPosition.x) + Math.Abs(_startingPoint.y - player.GridPosition.y)/10);
            var distanceCounterText = "Distance: "+ distance +"m";
            distanceCounter.text = distanceCounterText;

            currentAttack.text = player.CurrentAttack.DisplayName;
            currentConsumable.text = "Empty";
        }

        public void ShowWinScreen()
        {
            winScreen.SetActive(true);
        }
        
    }
}
