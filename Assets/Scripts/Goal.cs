using Maihem.Actors;
using Maihem.Managers;
using UnityEngine;

namespace Maihem
{
    public class Goal : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Player>() == null) return;
            animator.SetTrigger("GameOver");
            GameManager.Instance.GoalReached();
        }

        private void OnAnimationEnd()
        {
            GameManager.Instance.GameOver();
            gameObject.SetActive(false);
        }
    }
}
