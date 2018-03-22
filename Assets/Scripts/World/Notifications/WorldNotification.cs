using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class WorldNotification : MonoBehaviour
{
    private TextMesh textMesh;

    [Range(0.01f, 2f)]
    [SerializeField] private float duration = 2f;
    [SerializeField] private float animationVelocity = 5f;

    public void Awake()
    {
        textMesh = GetComponent<TextMesh>();
    }
    
    public void InitializeAndStart(Vector3 startingPosition, string text, float duration)
    {
        transform.position = startingPosition;
        textMesh.text = text;
        this.duration = duration;

        StartCoroutine(PlayEffect());
    }

    private IEnumerator PlayEffect()
    {
        float remaining = duration;

        Vector3 animationDirection = Vector3.forward;

        //Animation section 1
        while (remaining > duration / 2)
        {
            transform.position += (Vector3.up + animationDirection) * Time.deltaTime * animationVelocity;
            remaining -= Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
        //Animation section 2
        while (remaining > 0)
        {
            transform.position += (Vector3.down + animationDirection) * Time.deltaTime * animationVelocity;
            remaining -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        Destroy(this.gameObject);
    }
}
