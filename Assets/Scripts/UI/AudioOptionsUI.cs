using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maihem.UI
{
    public class AudioOptionsUI : MonoBehaviour
    {
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider effectsVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        
        [SerializeField] private TextMeshProUGUI masterVolumeValueText;
        [SerializeField] private TextMeshProUGUI effectsVolumeValueText;
        [SerializeField] private TextMeshProUGUI musicVolumeValueText;

        public void ResetVolumeValues()
        {
            UpdateMasterVolumeValue(1f);
            UpdateEffectsVolumeValue(1f);
            UpdateMusicVolumeValue(1f);

            masterVolumeSlider.value = 1f;
            effectsVolumeSlider.value = 1f;
            musicVolumeSlider.value = 1f;
        }

        public void UpdateMasterVolumeValue(float level)
        {
            masterVolumeValueText.text = $"{(int)(level * 100)}%";
        }
        
        public void UpdateEffectsVolumeValue(float level)
        {
            effectsVolumeValueText.text = $"{(int)(level * 100)}%";
        }
        
        public void UpdateMusicVolumeValue(float level)
        {
            musicVolumeValueText.text = $"{(int)(level * 100)}%";
        }
    }
}