using TMPro;
using UnityEngine;

namespace Maihem.Effects
{
    public class FloatingTextEffect : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textComponent;

        public void SetTextAndColor(string newText, Color newColor)
        {
            textComponent.color = newColor;
            textComponent.text = newText;
        } 
    }
}
