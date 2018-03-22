using UnityEngine;
using System.Collections;

public struct WorldNotificationArgs
{
    public Vector3 Position { get; }
    public string Text { get; }
    public float Duration { get; }
    public Color Color { get; }

    public WorldNotificationArgs(Vector3 position, string text, float duration)
    {
        this.Position = position;
        this.Text = text;
        this.Duration = duration;
        this.Color = Color.red;
    }
    
    public WorldNotificationArgs(Vector3 position, string text, float duration, Color color)
    {
        this.Position = position;
        this.Text = text;
        this.Duration = duration;
        this.Color = color;
    }
}

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
    
    public void InitializeAndStart(WorldNotificationArgs worldNotificationArgs)
    {
        transform.position = worldNotificationArgs.Position;
        textMesh.text = worldNotificationArgs.Text;
        //textMesh.color = worldNotificationArgs.Color;
        duration = worldNotificationArgs.Duration;
        
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
