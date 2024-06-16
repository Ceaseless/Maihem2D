using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maihem
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject healthBlipPrefab;
        [SerializeField] private float blipPadding = 0.01f;
        private List<GameObject> _activeBlips;

        private void Awake()
        {
            _activeBlips = new List<GameObject>();
        }

        public void SetMaxHealth(int maxHealth)
        {
            if (maxHealth <= _activeBlips.Count) return;
            var diff = maxHealth - _activeBlips.Count;
            for (var i = 0; i < diff; i++)
            {
                var newBlip = Instantiate(healthBlipPrefab, transform);
                newBlip.SetActive(false);
                _activeBlips.Add(newBlip);
            }
        }

        public void SetHealth(int newHealth)
        {
            if (newHealth < 0 || newHealth > _activeBlips.Count) return;
            var half = _activeBlips.Count / 2;
            for (var i = 0; i < newHealth; i++)
            {
                _activeBlips[i].transform.localPosition = new Vector3(-half*blipPadding + i * blipPadding, 0.5f, 0f);
                _activeBlips[i].SetActive(true);
            }

            for (var i = newHealth; i < _activeBlips.Count; i++)
            {
                _activeBlips[i].SetActive(false);
            }
        }
    }
}
