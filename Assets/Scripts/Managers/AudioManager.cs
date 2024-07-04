using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maihem.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Music Settings")] [SerializeField]
        private AudioSource musicSource;
        
        [Header("Sound Effects Settings")]
        [SerializeField] private float minimalTimeBetweenSfx;
        [SerializeField] private AudioSource soundFXSourcePrefab;
        [SerializeField] private int initialPoolSize;
        [SerializeField] private int poolGrowthStep;

        private List<AudioSource> _sfxSourcePool;
        private float _lastSfxTimestamp;
        
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
            
            InitializeSfxSourcePool();

        }

        private void InitializeSfxSourcePool()
        {
            _sfxSourcePool = new List<AudioSource>(initialPoolSize);
            for (var i = 0; i < initialPoolSize; i++)
            {
                var newSource = Instantiate(soundFXSourcePrefab, transform);
                _sfxSourcePool.Add(newSource);
            }
        }

        public void PlayMusic()
        {
            musicSource.Play();
        }

        public void PauseMusic()
        {
            musicSource.Pause();
        }

        public void ResetMusic()
        {
            musicSource.Stop();
            musicSource.Play();
        }

        public void FadeInMusic(float fadeTime)
        {
            StartCoroutine(FadeInAudioSource(musicSource, fadeTime));
        }

        public void PlaySoundFXDelayed(AudioClip audioClip, Vector3 sourcePosition, float volume, float delay)
        {
            StartCoroutine(DelayedSfx(audioClip, sourcePosition, volume, delay));
        }

        public void PlaySoundFX(AudioClip audioClip, Vector2Int cell, float volume)
        {
            var sourcePosition = MapManager.Instance.CellToWorld(cell);
            PlaySoundFX(audioClip, sourcePosition, volume);
        }

        public void PlaySoundFX(AudioClip audioClip, Vector3 sourcePosition, float volume)
        {
            if (!IsSfxAllowedToPlay())
            {
                PlaySoundFXDelayed(audioClip, sourcePosition, volume, minimalTimeBetweenSfx);
                return;
            }
            var source = GetSfxAudioSource();
            source.transform.position = sourcePosition;
            source.clip = audioClip;
            source.volume = volume;
            source.Play();
            _lastSfxTimestamp = Time.time;
        }

        private AudioSource GetSfxAudioSource()
        {
            foreach (var potentialSource in _sfxSourcePool)
            {
                if (!potentialSource.isPlaying) return potentialSource;
            }

            for (var i = 0; i < poolGrowthStep; i++)
            {
                var newSource = Instantiate(soundFXSourcePrefab, transform);
                _sfxSourcePool.Add(newSource);
            }

            return _sfxSourcePool[^1];
        }

        private IEnumerator FadeInAudioSource(AudioSource source, float fadeTime)
        {
            source.volume = 0f;
            source.Play();
            var time = 0f;
            while (source.volume < 1f)
            {
                source.volume = Mathf.Lerp(0f, 1f, time / fadeTime);
                time += Time.deltaTime;
                yield return null;
            }
            source.volume = 1f;
        }
        
        private IEnumerator FadeOutAudioSource(AudioSource source, float fadeTime)
        {
            source.volume = 1f;
            source.Play();
            var time = 0f;
            while (source.volume > 0f)
            {
                source.volume = Mathf.Lerp(1f, 0f, time / fadeTime);
                time += Time.deltaTime;
                yield return null;
            }
            source.volume = 0f;
        }
        
        private IEnumerator DelayedSfx(AudioClip audioClip, Vector3 sourcePosition, float volume, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlaySoundFX(audioClip, sourcePosition,volume);
        }
        
        

        private bool IsSfxAllowedToPlay()
        {
            return Time.time - _lastSfxTimestamp >= minimalTimeBetweenSfx;
        }
    }
}