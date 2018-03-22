using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBaseFSM : StateMachineBehaviour {

    public NPCBase Npc { get; set; }
    public Vector3 Waypoint;
    public float WaypointReachedRange { get; set; } = 1.0f; // The distance this has to be from the agent waypoint to reach it

}
