using UnityEngine;

namespace Maihem
{
    public class DestroyAfterAnimation : StateMachineBehaviour
    {
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
           Destroy(animator.gameObject, stateInfo.length);
        }

    }
}
