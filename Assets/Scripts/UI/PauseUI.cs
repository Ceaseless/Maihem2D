using System;
using Maihem.Managers;
using UnityEngine;
using UnityEngine.UI;
namespace Maihem.UI
{
    public class PauseUI : MonoBehaviour
    {
        
        [SerializeField] private GameObject pauseScreen;
        [SerializeField] private Button pauseResumeButton;
        [SerializeField] private Button pauseQuitButton;

        public void Initialize()
        {
            pauseResumeButton.onClick.AddListener(() => GameManager.Instance.PauseGame(this, EventArgs.Empty));
            pauseQuitButton.onClick.AddListener(() => GameManager.Instance.LoadMainMenu());
        }

        private void OnDestroy()
        {
            pauseResumeButton.onClick.RemoveAllListeners();
            pauseQuitButton.onClick.RemoveAllListeners();
        }

        public void ShowPauseUI(bool show)
        {
            pauseScreen.SetActive(show);
        }

        public void ResetState()
        {
            pauseScreen.SetActive(false);
        }

        public bool IsActive() => pauseScreen.activeInHierarchy;
    }
}
