using System.Collections.Generic;
using System.Linq;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Effects
{

    [System.Serializable]
    public class VisualEffectPoolSettings
    {
        public VisualEffectSettings visualEffect;
        public int initialPoolSize;
        public int poolGrowthStep;
    }
    
    public class VisualEffectsPool : MonoBehaviour
    {
        public static VisualEffectsPool Instance { get; private set;  }

        [SerializeField] private VisualEffectPoolSettings[] availableVisualEffects;
        private Dictionary<VisualEffectSettings, List<GameObject>> _effectsPool;
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
            _effectsPool = new Dictionary<VisualEffectSettings, List<GameObject>>(availableVisualEffects.Length);

            foreach (var effectSettings in availableVisualEffects)
            {
                var effect = effectSettings.visualEffect;
                if (_effectsPool.ContainsKey(effect))
                {
                    Debug.LogError("Duplicate effect type!");
                    continue;
                }

                var effectObjects = new List<GameObject>();
                for (var i = 0; i < effectSettings.initialPoolSize; i++)
                {
                    var tmp = Instantiate(effect.vfxPrefab, transform);
                    tmp.SetActive(false);
                    effectObjects.Add(tmp);
                }
                _effectsPool.Add(effect, effectObjects);
                
            }
        }

        
        
        public void PlayVisualEffect(VisualEffectSettings type, Vector3 position)
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
            var effectSettings = availableVisualEffects.First(a => a.visualEffect == type);
            var effectPrefab = effectSettings.visualEffect.vfxPrefab;
            for (var i = 0; i < effectSettings.poolGrowthStep; i++)
            {
                var tmp = Instantiate(effectPrefab, transform);
                tmp.SetActive(false);
                _effectsPool[type].Add(tmp);
            }
            
        }

        public void PlayVisualEffects(VisualEffectSettings type, IList<Vector3> positions)
        {
            if(!_effectsPool.ContainsKey(type)) return;
            foreach (var position in positions)
            {
                PlayVisualEffect(type,position);
            }
        }

        public void PlayVisualEffects(VisualEffectSettings type, IList<Vector2Int> positions)
        {
            var worldPositions = positions.Select(a => MapManager.Instance.CellToWorld(a));
            PlayVisualEffects(type, worldPositions as IList<Vector3>);
        }

        public void DisableAllEffects()
        {
            foreach (var (_, instances) in _effectsPool)
            {
                foreach (var effect in instances)
                {
                    effect.SetActive(false);
                }
            }
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
