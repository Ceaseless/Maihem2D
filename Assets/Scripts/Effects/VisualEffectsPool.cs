using System.Collections.Generic;
using UnityEngine;

namespace Maihem.Effects
{
    public enum VisualEffectType
    {
        Recover,
        Damage,
    }

    [System.Serializable]
    public class VisualEffectPair
    {
        public GameObject vfxPrefab;
        public VisualEffectType vfxType;
    }
    
    public class VisualEffectsPool : MonoBehaviour
    {
        public static VisualEffectsPool Instance { get; private set;  }

        [SerializeField] private VisualEffectPair[] availableVisualEffects;
        [SerializeField] private int initialPoolSize;
        [SerializeField] private int poolGrowthStep;
        private Dictionary<VisualEffectType, List<GameObject>> _effectsPool;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _effectsPool = new Dictionary<VisualEffectType, List<GameObject>>(availableVisualEffects.Length);

            foreach (var effect in availableVisualEffects)
            {
                if (_effectsPool.ContainsKey(effect.vfxType))
                {
                    Debug.LogError("Duplicate effect type!");
                    continue;
                }

                var effectObjects = new List<GameObject>();
                for (var i = 0; i < initialPoolSize; i++)
                {
                    var tmp = Instantiate(effect.vfxPrefab, transform);
                    tmp.SetActive(false);
                    effectObjects.Add(tmp);
                }
                _effectsPool.Add(effect.vfxType, effectObjects);
                
            }
        }

        public void PlayVisualEffect(VisualEffectType type, Vector3 position)
        {
            if (!_effectsPool.TryGetValue(type, out var effectInstances)) return;

            foreach (var effect in effectInstances)
            {
                if (!effect.gameObject.activeInHierarchy)
                {
                    effect.transform.position = position;
                    effect.SetActive(true);
                    // Play effect
                    // effect.SetActive(false)
                    return;
                }
            }
            // Grow pool
            
        }

        // public TargetMarker GetMarker()
        // {
        //     foreach (var marker in _markerPool)
        //     {
        //         if (!marker.gameObject.activeInHierarchy) return marker;
        //     }
        //
        //     for (var i = 0; i < poolGrowthStep; i++)
        //     {
        //         var tmp = Instantiate(markerPrefab, transform);
        //         tmp.SetActive(false);
        //         _markerPool.Add(tmp.GetComponent<TargetMarker>());
        //     }
        //
        //     return _markerPool[^1];
        // }

        // public List<TargetMarker> GetMarkers(int count)
        // {
        //     var markers = new List<TargetMarker>(count);
        //     foreach (var marker in _markerPool)
        //     {
        //         if (marker.gameObject.activeInHierarchy) continue;
        //         markers.Add(marker);
        //         if (markers.Count == count) return markers;
        //     }
        //
        //     if (markers.Count >= count) return markers;
        //     
        //     
        //     var diff = count - markers.Count;
        //     for (var i = 0; i < diff; i++)
        //     {
        //         var tmp = Instantiate(markerPrefab, transform);
        //         tmp.SetActive(false);
        //         var markerComponent = tmp.GetComponent<TargetMarker>();
        //         _markerPool.Add(markerComponent);
        //         markers.Add(markerComponent);
        //     }
        //
        //     return markers;
        // }
        //
        // public void HideAllMarkers()
        // {
        //     foreach (var marker in _markerPool)
        //     {
        //         marker.HideMarker();
        //     }
        // }
    }
}
