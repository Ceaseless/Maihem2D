using System.Collections.Generic;
using UnityEngine;

namespace Maihem.UI
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject healthBlipPrefab;
        [SerializeField] private float blipHorizontalPadding = 0.01f;
        [SerializeField] private float blipVerticalPadding = 0.5f;
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
            
            var blipsPerRow = (int)(1 / blipHorizontalPadding);
            var half = blipsPerRow / 2;
            var row = 0;
            var count = 0;
            for (var i = 0; i < newHealth; i++)
            {
                if (count > blipsPerRow)
                {
                    row++;
                    count = 0;
                }
                _activeBlips[i].transform.localPosition = new Vector3(-half*blipHorizontalPadding + count * blipHorizontalPadding, 0.5f+row*blipVerticalPadding, 0f);
                _activeBlips[i].SetActive(true);
                count++;


            }

            for (var i = newHealth; i < _activeBlips.Count; i++)
            {
                _activeBlips[i].SetActive(false);
            }
        }
        
        public void HideHealth() {}
    }
}
