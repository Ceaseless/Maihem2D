using Maihem.Extensions;
using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {

        [SerializeField] private float panDuration = 0.25f;
        private bool _isMoving;
        private float _moveProgress;
        private Vector3 _moveStart, _moveTarget;
        private Camera _camera;
        private float _cameraWidth, _cameraHeight, _halfCameraWidth, _halfCameraHeight;
        private void Start()
        {
            _camera = GetComponent<Camera>();
            _cameraHeight = 2f * _camera.orthographicSize;
            _cameraWidth = _cameraHeight * _camera.aspect;
            _halfCameraWidth = _cameraWidth * 0.5f;
            _halfCameraHeight = _cameraHeight * 0.5f;
        }

        public void Reset()
        {
            _isMoving = false;
            _moveProgress = 0f;
            transform.position = new Vector3(21f, 0f, -10f); // TODO: Make dynamic
        }

        public void UpdateCameraScroll()
        {
            if(GameManager.Instance.Player.transform.position.x - transform.position.x > 5f)
            { 
                TranslateCamera(Vector3.right * 1f);
            }
            else
            {
                TranslateCamera(Vector3.right * 0.5f);
            }
        }
    

        public void TranslateCamera(Vector2 translation)
        {
            //if (_isMoving) return;
            _moveStart = transform.position;
            _moveTarget = _moveStart + translation.WithZ(0);
            _moveProgress = 0f;
            _isMoving = true;
        }
        
    
        public bool IsPositionOffScreen(Vector3 position)
        {
            var camPosition = _isMoving ? _moveTarget : transform.position;
            var bounds = new Bounds(camPosition.WithZ(0), new Vector3(_cameraWidth, _cameraHeight, 1f));
            return !bounds.Contains(position.XY());
        }

        private void LateUpdate()
        {
            if (!_isMoving) return;
            _moveTarget = _moveTarget.WithY(GameManager.Instance.Player.transform.position.y);
            if (_moveProgress >= 1f)
            {
                _isMoving = false;
                _moveProgress = 0f;
                transform.position = _moveTarget;
                return;
            }
        
            transform.position = Vector3.Lerp(_moveStart, _moveTarget, _moveProgress / panDuration);
            _moveProgress += Time.deltaTime;
        }
    }
}