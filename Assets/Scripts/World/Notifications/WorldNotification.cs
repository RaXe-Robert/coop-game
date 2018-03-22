using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class WorldNotification : MonoBehaviour
{
    private TextMesh textMesh;
    
    private float duration;
    private float animationVelocity = 6f;

    public void Awake()
    {
        textMesh = GetComponent<TextMesh>();
    }
    
    public void InitializeAndStart(Vector3 startingPosition, string text, float duration)
    {
        transform.position = startingPosition;
        textMesh.text = text;
        this.duration = duration;

        StartCoroutine(PlayDefaultAnimation());
    }

    private IEnumerator PlayDefaultAnimation()
    {
        float remaining = duration;

        Vector3 animationDirection = Vector3.forward;

        while (remaining > 0)
        {
            transform.position += (Vector3.up + animationDirection) * Time.deltaTime * animationVelocity;
            remaining -= Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
        
        Destroy(this.gameObject);
    }
}
