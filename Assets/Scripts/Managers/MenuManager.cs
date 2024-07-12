using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maihem.Managers
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject infoWindow;
        public static bool TutorialActivated { get; private set; }

        private void Start()
        {
            AudioManager.Instance.FadeInMusic(2f);
        }

        public void StartGame(bool tutorial)
        {
            TutorialActivated = tutorial;
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
    }
}
