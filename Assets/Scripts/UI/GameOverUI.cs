using System;
using UnityEngine;

namespace Maihem.UI
{
    public class GameOverUI : MonoBehaviour
    {
        public enum GameOverReason
        {
            Light, Health, Win
        }
        
        [SerializeField] private GameObject winScreen;
        [SerializeField] private GameObject playerBeatenMsg;
        [SerializeField] private GameObject playerWonMsg;
        [SerializeField] private GameObject playerLightMsg;


        public void ShowGameOverUI(GameOverReason reason)
        {
            if (IsActive()) return;
            switch (reason)
            {
                case GameOverReason.Light:
                    LightGameOver();
                    break;
                case GameOverReason.Health:
                    HealthGameOver();
                    break;
                case GameOverReason.Win:
                    WinGameOver();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(reason), reason, null);
            }
            winScreen.SetActive(true);
        }
        

        public void ResetState()
        {
            winScreen.SetActive(false);
            playerWonMsg.SetActive(false);
            playerBeatenMsg.SetActive(false);
            playerLightMsg.SetActive(false);
        }
        
        private void LightGameOver()
        {
            playerWonMsg.SetActive(false);
            playerBeatenMsg.SetActive(false);
            playerLightMsg.SetActive(true);
        }
        
        private void HealthGameOver()
        {
            playerWonMsg.SetActive(false);
            playerLightMsg.SetActive(false);
            playerBeatenMsg.SetActive(true);
        }

        private void WinGameOver()
        {
            playerLightMsg.SetActive(false);
            playerBeatenMsg.SetActive(false);
            playerWonMsg.SetActive(true);
        }

        public bool IsActive() => winScreen.activeInHierarchy;
    }
}