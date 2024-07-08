using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    public class KillZoneController : MonoBehaviour
    {
        [SerializeField] private float playerMaxDistance = 20f;
        [Min(0f)]
        [SerializeField] private float passiveMoveSpeed = 0.5f;
        [Min(0f)]
        [SerializeField] private float activeMoveSpeed = 1f;

        public bool stopped;
        private void Start()
        {
            Reset();
        }

        public void Reset()
        {
            stopped = false;
            transform.position = Vector3.zero;
        }
        
        public void UpdateBounds()
        {
            if (stopped) return;
            var player = GameManager.Instance.Player;
            var speed = player.transform.position.x - transform.position.x > playerMaxDistance
                ? activeMoveSpeed
                : passiveMoveSpeed;
            
            transform.Translate(Vector3.right * speed);
        }
        

       
    }
}
