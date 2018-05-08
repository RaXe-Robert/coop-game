using UnityEngine;

public class Tool : Item
{
    public Tool(ToolData toolData) : base(toolData)
    {
        ToolType = toolData.ToolType;
    }

    public ToolType ToolType { get; }
}