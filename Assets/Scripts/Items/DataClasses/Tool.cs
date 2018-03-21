using UnityEngine;

public class Tool : ItemBase
{
    public Tool(ToolData toolData) : base(toolData)
    {
        toolType = toolData.ToolType;
    }

    [SerializeField] private ToolType toolType;
    public ToolType ToolType { get { return toolType; } }

    public override void Equip()
    {
        base.Equip();
    }
}