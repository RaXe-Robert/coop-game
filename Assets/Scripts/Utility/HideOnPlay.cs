using UnityEngine;
using System.Collections;

public class HideOnPlay : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
