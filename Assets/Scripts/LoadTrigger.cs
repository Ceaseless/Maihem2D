using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maihem
{
    public class LoadTrigger : MonoBehaviour
    {
        public float LoadProgress { get; private set; }

        private void Start()
        {
            StartCoroutine(LoadSceneAsync());
        }

        private IEnumerator LoadSceneAsync()
        {
            var loadHandle = SceneManager.LoadSceneAsync(SceneLoadingData.SceneToLoad.ToString());
            if (loadHandle == null)
            {
                Debug.LogError($"[Load Trigger]: Invalid scene name when loading {SceneLoadingData.SceneToLoad.ToString()}");
                yield break;
            }
            loadHandle.allowSceneActivation = false;
            while (!loadHandle.isDone)
            {
                LoadProgress = loadHandle.progress;
                if (loadHandle.progress >= 0.9f)
                {
                    loadHandle.allowSceneActivation = true;
                    
                }
                yield return null;
            }
        }
    }
}