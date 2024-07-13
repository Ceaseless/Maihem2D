using System.Collections.Generic;
using UnityEngine;

namespace Maihem
{
    public class MarkerPool : MonoBehaviour
    {
        public static MarkerPool Instance { get; private set;  }

        [SerializeField] private GameObject markerPrefab;
        [SerializeField] private int initialPoolSize;
        [SerializeField] private int poolGrowthStep;
        private List<TargetMarker> _markerPool;
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
            _markerPool = new List<TargetMarker>(initialPoolSize);
            for (var i = 0; i < initialPoolSize; i++)
            {
                var tmp = Instantiate(markerPrefab, transform);
                tmp.SetActive(false);
                _markerPool.Add(tmp.GetComponent<TargetMarker>());
            }
        }

        public TargetMarker GetMarker()
        {
            foreach (var marker in _markerPool)
            {
                if (!marker.gameObject.activeInHierarchy) return marker;
            }

            for (var i = 0; i < poolGrowthStep; i++)
            {
                var tmp = Instantiate(markerPrefab, transform);
                tmp.SetActive(false);
                _markerPool.Add(tmp.GetComponent<TargetMarker>());
            }

            return _markerPool[^1];
        }

        public List<TargetMarker> GetMarkers(int count)
        {
            if(count == 0) return new List<TargetMarker>(0);
            var markers = new List<TargetMarker>(count);
            foreach (var marker in _markerPool)
            {
                if (marker.gameObject.activeInHierarchy) continue;
                markers.Add(marker);
                if (markers.Count == count) return markers;
            }

            if (markers.Count >= count) return markers;
            
            
            var diff = count - markers.Count;
            for (var i = 0; i < diff; i++)
            {
                var tmp = Instantiate(markerPrefab, transform);
                tmp.SetActive(false);
                var markerComponent = tmp.GetComponent<TargetMarker>();
                _markerPool.Add(markerComponent);
                markers.Add(markerComponent);
            }

            return markers;
        }

        public void HideAllMarkers()
        {
            foreach (var marker in _markerPool)
            {
                marker.DisableMarker();
            }
        }
    }
}
