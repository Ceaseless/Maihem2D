using Maihem.SceneLoading;
using UnityEngine;
using UnityEngine.UI;

namespace Maihem.UI
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] private LoadTrigger loadTrigger;
        [SerializeField] private Image loadingBar;
        [SerializeField] private float loadingBarFudgeThreshold = 0.89f;

        private void LateUpdate()
        {
            var progress = loadTrigger.LoadProgress >= loadingBarFudgeThreshold ? 1f : loadTrigger.LoadProgress;
            loadingBar.fillAmount = 1f-progress;
        }
    }
}
