using Maihem.Actors;
using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    public class Goal : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Player>() == null) return;
            GameManager.Instance.GameOver();
        }
    }
}
