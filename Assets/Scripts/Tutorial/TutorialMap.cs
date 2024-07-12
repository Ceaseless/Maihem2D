using Maihem.Managers;
using UnityEngine;

namespace Maihem.Tutorial
{
    public class TutorialMap : MonoBehaviour
    {
        private void Awake()
        {
            GameManager.Instance.ToggleKillZone(true);
            GameManager.Instance.TogglePlayerStats(false);
        }
        
    }
}
