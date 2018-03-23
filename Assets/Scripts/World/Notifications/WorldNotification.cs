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
    
    public void InitializeAndStart(WorldNotificationArgs worldNotificationArgs)
    {
        transform.position = worldNotificationArgs.Position;
        textMesh.text = worldNotificationArgs.Text;
        textMesh.color = GetColorByName(worldNotificationArgs.Color);
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

    private Color GetColorByName(string color)
    {
        switch (color)
        {
            case "cyan":
                return Color.cyan;
            case "green":
                return Color.green;
            case "red":
                return Color.red;
            case "yellow":
                return Color.yellow;
            case "blue":
                return Color.blue;
            case "magenta":
                return Color.magenta;
            case "gray":
                return Color.gray;
            case "white":
                return Color.white;
            case "grey":
                return Color.grey;
            //case "black":
            //case "clear":
            default:
                return Color.black;
        }
    }
}
