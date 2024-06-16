using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maihem
{
    public class DestroyAfterAnimation : StateMachineBehaviour
    {
        
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
           Destroy(animator.gameObject, stateInfo.length);
        }

    }
}
