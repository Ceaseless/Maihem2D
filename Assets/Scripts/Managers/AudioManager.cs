using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maihem.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

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
            if (!IsAllowedToPlay())
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

        private IEnumerator DelayedSfx(AudioClip audioClip, Vector3 sourcePosition, float volume, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlaySoundFX(audioClip, sourcePosition,volume);
        }

        private bool IsAllowedToPlay()
        {
            return Time.time - _lastSfxTimestamp >= minimalTimeBetweenSfx;
        }
    }
}