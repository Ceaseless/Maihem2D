using System;
using System.Collections.Generic;
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
        [SerializeField] private Image currentAttack;
        [SerializeField] private Image currentConsumable;
        
        [SerializeField] private List<Sprite> attackSprites;

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

            switch(player.CurrentAttack.DisplayName)
            {
                case "Kick":
                    currentAttack.sprite = attackSprites[0];
                    break;
                case "Slam":
                    currentAttack.sprite = attackSprites[1];
                    break;
                case "Ranged":
                    currentAttack.sprite = attackSprites[2];
                    break;
                case "Stomp":
                    currentAttack.sprite = attackSprites[3];
                    break;
                case null:
                    currentAttack = null;
                    break;
            }

            currentConsumable.sprite = player.currentConsumable.sprite;
            
        }

        public void ShowWinScreen()
        {
            winScreen.SetActive(true);
        }
        
    }
}
