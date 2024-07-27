using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Maihem.UI
{
    public class AudioOptionsUI : MonoBehaviour
    {
        [SerializeField] private AudioMixer affectedMixer;
        
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider effectsVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        
        [SerializeField] private TextMeshProUGUI masterVolumeValueText;
        [SerializeField] private TextMeshProUGUI effectsVolumeValueText;
        [SerializeField] private TextMeshProUGUI musicVolumeValueText;

        private const string MasterVolumeParameter = "masterVolume";
        private const string EffectVolumeParameter = "effectsVolume";
        private const string MusicVolumeParameter = "musicVolume";
        
        private void Start()
        {
            SynchronizeUIWithAudioMixer();
        }

        private void SynchronizeUIWithAudioMixer()
        {
            if (affectedMixer.GetFloat(MasterVolumeParameter, out var masterVolume))
            {
                masterVolume = Mathf.Pow(10f,(masterVolume / 20f));
                UpdateMasterVolumeValue(masterVolume);
                masterVolumeSlider.value = masterVolume;
            }
            if (affectedMixer.GetFloat(EffectVolumeParameter, out var effectsVolume))
            {
                effectsVolume = Mathf.Pow(10f,(effectsVolume / 20f));
                UpdateEffectsVolumeValue(effectsVolume);
                effectsVolumeSlider.value = effectsVolume;
            }
            if (affectedMixer.GetFloat(MusicVolumeParameter, out var musicVolume))
            {
                musicVolume = Mathf.Pow(10f,(musicVolume / 20f));
                UpdateMusicVolumeValue(musicVolume);
                musicVolumeSlider.value = musicVolume;
            }
        }

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