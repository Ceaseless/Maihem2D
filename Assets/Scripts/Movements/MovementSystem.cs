using System.Collections.Generic;
using Maihem.Actors;
using Maihem.Effects;
using Maihem.Extensions;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Movements
{
    public class MovementSystem : MonoBehaviour
    {
        [SerializeField] private float waitChanceWhileIdle = 0.5f;
        [SerializeField] private MovementStrategy currentStrategy;
        [SerializeField] private VisualEffectSettings detectionEffect;
        [SerializeField] private AudioClip detectionSoundEffect;
        private bool _isActivated;

        private Actor _parentActor;

        private void Awake()
        {
            _parentActor = GetComponent<Actor>();
            _isActivated = false;
        }

        private void CheckAlert()
        {
            if (_isActivated) return;
            if (currentStrategy.CheckAlert(_parentActor.GridPosition))
            {
                _isActivated = true;
                var effectPosition = _parentActor.transform.position + Vector3.up;

                VisualEffectsPool.Instance.PlayVisualEffect(detectionEffect, effectPosition);
                AudioManager.Instance.PlaySoundFX(detectionSoundEffect, effectPosition, 1f);
            }
        }

        public bool TryMove(int range, out List<Vector2Int> path)
        {
            CheckAlert();
            if (!_isActivated)
            {
                // Wait instead of trying to idle move
                if (waitChanceWhileIdle >= Random.Range(0, 1f))
                {
                    path = null;
                    return false;
                }

                // Idle move found a position
                if (MovementStrategy.TryIdleMove(_parentActor.GridPosition,
                        _parentActor.CurrentFacing.GetFacingVector(), out path)) return true;

                // Idle move didn't find a position -> Wait
                path = null;
                return false;
            }

            // We are active -> Move
            path = currentStrategy.ActivatedMove(_parentActor.GridPosition, range);
            return true;
        }
    }
}