using UnityEngine;

namespace Maihem.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource soundFXSource;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlaySoundFX(AudioClip audioClip, Vector2Int cell, float volume)
        {
            var sourcePosition = MapManager.Instance.CellToWorld(cell);
            PlaySoundFX(audioClip, sourcePosition, volume);
        }

        public void PlaySoundFX(AudioClip audioClip, Vector3 sourcePosition, float volume)
        {
            var source = Instantiate(soundFXSource, sourcePosition, Quaternion.identity);
            source.clip = audioClip;
            source.volume = volume;
            source.Play();
            var clipLength = audioClip.length;
            Destroy(source.gameObject, clipLength);
        }
    }
}