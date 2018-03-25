using UnityEngine;

public struct WorldNotificationArgs
{
    public Vector3 Position { get; }
    public string Text { get; }
    public float Duration { get; }
    public string Color { get; }
    
    public WorldNotificationArgs(Vector3 position, string text)
    {
        this.Position = position;
        this.Text = text;
        this.Duration = 0.4f;
        this.Color = "black";
    }

    public WorldNotificationArgs(Vector3 position, string text, float duration)
    {
        this.Position = position;
        this.Text = text;
        this.Duration = duration;
        this.Color = "black";
    }

    public WorldNotificationArgs(Vector3 position, string text, float duration, string color)
    {
        this.Position = position;
        this.Text = text;
        this.Duration = duration;
        this.Color = color;
    }
}
