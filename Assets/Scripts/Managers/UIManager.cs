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
        [SerializeField] private TextMeshProUGUI attackCost;
        [SerializeField] private Image currentConsumable;
        
        [SerializeField] private List<Sprite> attackSprites;

        [SerializeField] private GameObject playerStats;
        
        [SerializeField] private GameObject winScreen;
        [SerializeField] private GameObject playerBeatenMsg;
        [SerializeField] private GameObject playerWonMsg;
        [SerializeField] private GameObject playerLightMsg;

        [SerializeField] private GameObject pauseScreen;
        [SerializeField] private Button pauseResumeButton;
        [SerializeField] private Button pauseQuitButton;
        
        private Vector2Int _startingPoint;
        
        
        public void Initialize()
        {
            pauseResumeButton.onClick.AddListener(() => GameManager.Instance.PauseGame(this, EventArgs.Empty));
            pauseQuitButton.onClick.AddListener(() => GameManager.Instance.Exit());
            
            ResetState();
        }

        public void ResetState()
        {
            _startingPoint = GameManager.Instance.Player.GridPosition;
            GameManager.Instance.Player.OnStatusUpdate += UpdateStatusUI;
            winScreen.SetActive(false);
            pauseScreen.SetActive(false);

            playerWonMsg.SetActive(true);
            playerBeatenMsg.SetActive(false);
            playerLightMsg.SetActive(false);
            
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

        public void ShowGameOverScreen()
        {
            winScreen.SetActive(true);
        }

        public void PlayerInLight()
        {
            if (playerWonMsg.activeSelf)
            {
                playerWonMsg.SetActive(false);
            }
            if (playerBeatenMsg.activeSelf)
            {
                playerBeatenMsg.SetActive(false);
            }
            playerLightMsg.SetActive(true);
        }

        public void PlayerBeaten()
        {
            if (playerWonMsg.activeSelf)
            {
                playerWonMsg.SetActive(false);
            }
            if (playerLightMsg.activeSelf)
            {
                playerLightMsg.SetActive(false);
            }
            playerBeatenMsg.SetActive(true);
        }
        public void TogglePlayerStats(bool active)
        {
            playerStats.SetActive(active);
        }

        public bool TrySetPauseMenuActive(bool show)
        {
            if (winScreen.activeSelf || show == pauseScreen.activeInHierarchy) return false;
            pauseScreen.SetActive(show);
            return true;
        }
        
    }
   
        
    }

