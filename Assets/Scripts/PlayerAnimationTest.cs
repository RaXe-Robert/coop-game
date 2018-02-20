using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour
{

    Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine("switchAnimator");
    }

    IEnumerator switchAnimator()
    {
        while (true)
        {
            bool currentState = animator.GetBool("IsRunning");
            animator.SetBool("IsRunning", !currentState);
            yield return new WaitForSeconds(5);
        }
    }
}
