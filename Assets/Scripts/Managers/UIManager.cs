using System;
using System.Collections.Generic;
using Maihem.UI;
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
        [SerializeField] private Animator attackAnimator;
        [SerializeField] private TextMeshProUGUI attackCost;
        [SerializeField] private Image currentConsumable;
        [SerializeField] private Animator consumableAnimator;
        
        [SerializeField] private List<Sprite> attackSprites;

        [SerializeField] private GameObject playerStats;

        [SerializeField] private GameOverUI gameOverUI;
        [SerializeField] private PauseUI pauseUI;
       
        
        private Vector2Int _startingPoint;
        
        
        public void Initialize()
        {
            // pauseResumeButton.onClick.AddListener(() => GameManager.Instance.PauseGame(this, EventArgs.Empty));
            // pauseQuitButton.onClick.AddListener(() => GameManager.Instance.LoadMainMenu());
            pauseUI.Initialize();
            ResetState();
        }

        public void ResetState()
        {
            _startingPoint = GameManager.Instance.Player.GridPosition;
            GameManager.Instance.Player.OnStatusUpdate += UpdateStatusUI;
            gameOverUI.ResetState();
            pauseUI.ResetState();
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

            attackCost.text = player.CurrentAttack.StaminaCost.ToString();
            currentConsumable.sprite = player.currentConsumable.sprite;
            
        }

        public void ShowGameOverUI(GameOverUI.GameOverReason reason)
        {
            gameOverUI.ShowGameOverUI(reason);
        }
        
        public void TogglePlayerStats(bool active)
        {
            playerStats.SetActive(active);
        }

        public bool TrySetPauseMenuActive(bool show)
        {
            if (gameOverUI.IsActive() || show == pauseUI.IsActive()) return false;
            pauseUI.ShowPauseUI(show);
            return true;
        }

        public void ItemButtonFlash(string color)
        {
            consumableAnimator.SetTrigger(color);
        }
        public void AttackButtonFlash(string color)
        {
            attackAnimator.SetTrigger(color);
        }
    }
   
        
    }

