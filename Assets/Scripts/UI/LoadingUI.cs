using UnityEngine;
using UnityEngine.UI;

namespace Maihem.UI
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] private LoadTrigger loadTrigger;
        [SerializeField] private Image loadingBar;

        private void LateUpdate()
        {
            loadingBar.fillAmount = loadTrigger.LoadProgress;
        }
    }
}
