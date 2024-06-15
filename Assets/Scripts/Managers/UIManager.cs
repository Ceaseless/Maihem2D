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
        
        private Vector2Int _startingPoint;
        
        public void Initialize()
        {
            _startingPoint = GameManager.Instance.Player.GridPosition;
            GameManager.Instance.Player.OnStatusUpdate += UpdateStatusUI;
            ChangeConsumable("Empty");
            UpdateStatusUI(this, EventArgs.Empty);
        }
        
        private void UpdateStatusUI(object sender, EventArgs e)
        {
            var player = GameManager.Instance.Player;
            
            healthBar.maxValue = player.MaxHealth;
            healthBar.value = player.CurrentHealth;
            var healthValueText = player.CurrentHealth + "/" + player.MaxHealth + " HP";
            healthValue.text = healthValueText.PadLeft(8,' ');
            
            staminaBar.maxValue = player.MaxStamina;
            staminaBar.value = player.CurrentStamina;
            var staminaValueText = player.CurrentStamina + "/" + player.MaxStamina;
            staminaValue.text = staminaValueText.PadLeft(8,' ');

            //var distance = Math.Abs(_startingPoint.x - player.GridPosition.x);
            var distance = Math.Abs(Math.Abs(_startingPoint.x - player.GridPosition.x) + Math.Abs(_startingPoint.y - player.GridPosition.y)/10);
            var distanceCounterText = "Distance: "+ distance +"m";
            distanceCounter.text = distanceCounterText.PadRight(16, ' ');

            currentAttack.text = player.CurrentAttack.DisplayName;
        }
        
        
        public void ChangeConsumable(string consumableName)
        {
            currentConsumable.text = consumableName;
        }
    }
}
