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
        private List<GameObject> _markerPool;
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
            _markerPool = new List<GameObject>();
            for (var i = 0; i < initialPoolSize; i++)
            {
                var tmp = Instantiate(markerPrefab, transform);
                tmp.SetActive(false);
                _markerPool.Add(tmp);
            }
        }

        public GameObject GetMarker()
        {
            foreach (var marker in _markerPool)
            {
                if (!marker.gameObject.activeInHierarchy) return marker;
            }

            for (var i = 0; i < poolGrowthStep; i++)
            {
                var tmp = Instantiate(markerPrefab, transform);
                tmp.SetActive(false);
                _markerPool.Add(tmp);
            }

            return _markerPool[^1];
        }

        public List<GameObject> GetMarkers(int count)
        {
            var markers = new List<GameObject>(count);
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
                _markerPool.Add(tmp);
                markers.Add(tmp);
            }

            return markers;
        }
    }
}
