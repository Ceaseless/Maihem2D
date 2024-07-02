using System.Collections;
using UnityEngine;

namespace Maihem
{
    public class ProjectileEffect : MonoBehaviour
    {
        public void LaunchFromTo(Vector3 start, Vector3 end, float duration)
        {
            StartCoroutine(ProjectileAnimation(start, end, duration));
        }
        
        private IEnumerator ProjectileAnimation(Vector3 start, Vector3 end, float duration)
        {
           
            var time = 0f;
            while (time < duration)
            {
                transform.position = Vector3.Lerp(start, end, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = end;
            Destroy(gameObject);
        }
    }
}