using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Maihem.Managers
{
    public class MenuManager : MonoBehaviour
    {
        public enum MenuButton
        {
            StartGame,
            Tutorial,
            Info,
            Exit
        }
        
        [SerializeField] private GameObject infoWindow;
        [SerializeField] private Button startButton;
        [SerializeField] private Button tutorialButton;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private float delayTime = 0.5f;
        [SerializeField] private AudioClip selectSound;
        public static bool TutorialActivated { get; private set; }
        private Action _delayedAction;
        private void Start()
        {
            AudioManager.Instance.FadeInMusic(2f);
            startButton.onClick.AddListener(() => ClickButton(MenuButton.StartGame));
            tutorialButton.onClick.AddListener(() => ClickButton(MenuButton.Tutorial));
            infoButton.onClick.AddListener(() => ClickButton(MenuButton.Info));
            exitButton.onClick.AddListener(() => ClickButton(MenuButton.Exit));
            
        }

        private void ClickButton(MenuButton clickedButton)
        {
            _delayedAction = clickedButton switch
            {
                MenuButton.StartGame => StartGame,
                MenuButton.Tutorial => StartTutorial,
                MenuButton.Info => ToggleInfo,
                MenuButton.Exit => Exit,
                _ => throw new ArgumentOutOfRangeException(nameof(clickedButton), clickedButton, null)
            };
            AudioManager.Instance.PlaySoundFX(selectSound, Vector3.zero, 1f);
            StartCoroutine(DelayAction());
        }

        private void StartGame()
        {
            TutorialActivated = false;
            LoadGameScene();
        }

        private void StartTutorial()
        {
            TutorialActivated = true;
            LoadGameScene();
            
        }

        private void LoadGameScene()
        {
            SceneLoadingData.SceneToLoad = SceneLoadingData.LoadableScene.GameScene;
            SceneManager.LoadScene(SceneLoadingData.LoadableScene.LoadingScene.ToString());
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void ToggleInfo()
        {
            infoWindow.SetActive(!infoWindow.activeSelf);
        }

        private IEnumerator DelayAction()
        {
            yield return new WaitForSeconds(delayTime);
            _delayedAction.Invoke();
        }
        
    }
}
