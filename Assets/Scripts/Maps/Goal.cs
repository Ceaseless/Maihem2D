using Maihem.Actors;
using Maihem.Managers;
using UnityEngine;

namespace Maihem.Maps
{
    public class Goal : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private AudioClip victory;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Player>() == null) return;
            animator.SetTrigger("GameOver");
            GameManager.Instance.GoalReached();
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlaySoundFX(victory, MapManager.Instance.WorldToCell(transform.position), 1f);
        }

        private void OnAnimationEnd()
        {
            GameManager.Instance.GameOver();
            gameObject.SetActive(false);
        }
    }
}
