using UnityEngine;
using System.Collections;

public class DestroyOnOwnerLeave : Photon.MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (photonView != null)
        {
            if (photonView.owner == null)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogError("DestroyOnOwnerLeave must be attached to a photonView");
            this.enabled = false;
        }
    }
}
