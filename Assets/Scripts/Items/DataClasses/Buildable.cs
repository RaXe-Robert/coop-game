public class Buildable : ItemBase
{
    public float Recoverable { get; }

    public Buildable(BuildableData buildableData) : base(buildableData)
    {
        Recoverable = buildableData.Recoverable;
    }
}