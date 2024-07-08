using System.Collections;
using System.Collections.Generic;
using Maihem.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maihem
{
    public class MenuManager : MonoBehaviour
    {
        public static bool TutorialActivated { get; private set; }
        public void StartGame(bool tutorial)
        {
            TutorialActivated = tutorial;
            SceneManager.LoadScene("GameScene");
        }
    }
}
