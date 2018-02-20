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
        StartCoroutine(SwitchAnimator());
    }

    IEnumerator SwitchAnimator()
    {
        var timeout = new WaitForSeconds(5);
        while (true)
        {
            bool currentState = animator.GetBool("IsRunning");
            animator.SetBool("IsRunning", !currentState);
            yield return timeout;
        }
    }
}
