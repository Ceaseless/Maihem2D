using Maihem.Managers;
using UnityEngine;

namespace Maihem.Tutorial
{
    public class EnableKillZone : MonoBehaviour
    {
        private bool _triggered;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_triggered) return;
            MapManager.Instance.TutorialFinished();
            GameManager.Instance.ToggleKillZone(false);
            GameManager.Instance.ResetGame();
            _triggered = true;
        }
    }
}
