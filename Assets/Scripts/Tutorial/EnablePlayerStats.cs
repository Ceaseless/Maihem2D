using Maihem.Managers;
using UnityEngine;

namespace Maihem.Tutorial
{
    public class EnablePlayerStats : MonoBehaviour
    {
        private bool _triggered;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_triggered) return;
            GameManager.Instance.TogglePlayerStats(true);
            _triggered = true;

        }
    }
}
