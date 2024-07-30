using TMPro;
using UnityEngine;

namespace Maihem.Attacks.AttackMarkers
{
    public class TargetMarker : MonoBehaviour
    {
        [SerializeField] private TextMeshPro markerText;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void SetMarkerVisuals(Color newColor, string newText)
        {
            spriteRenderer.color = newColor;
            markerText.text = newText;
        }
        
        public void SetMarkerColor(Color newColor)
        {
            spriteRenderer.color = newColor;
        }

        public void SetMarkerText(string newText)
        {
            markerText.text = newText;
        }
        
        public void ShowMarker()
        {
            gameObject.SetActive(true);
        }

        public void DisableMarker()
        {
            gameObject.SetActive(false);
        }
    }
}
