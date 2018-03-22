using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBaseFSM : StateMachineBehaviour {

    public NPCBase Npc { get; set; }
    public Vector3 Waypoint;
    public float Range { get; set; } = 1.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
