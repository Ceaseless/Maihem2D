using System.Collections;
using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maihem
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject infoWindow;
        public static bool TutorialActivated { get; private set; }
        public void StartGame(bool tutorial)
        {
            TutorialActivated = tutorial;
            SceneManager.LoadScene("GameScene");
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void ToggleInfo()
        {
            if (!infoWindow.activeSelf)
            {
                infoWindow.SetActive(true);
                return;
            }
            infoWindow.SetActive(false);
        }
    }
}
