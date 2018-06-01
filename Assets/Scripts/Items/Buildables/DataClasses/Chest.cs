using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chest : BuildableWorldObject
{
    private Animator animator;
    public bool IsOpened { get; private set; } = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override UnityAction[] InitializeActions()
    {
        return new UnityAction[]
        {
            new UnityAction(OpenChest),
            new UnityAction(CloseChest)
        };
    }

    private void OpenChest()
    {
        Debug.Log("Opening");
        IsOpened = true;
        animator.SetBool("IsOpen", true);
    }

    private void CloseChest()
    {
        Debug.Log("Closing");
        IsOpened = false;
        animator.SetBool("IsOpen", false);
    }

   
}
