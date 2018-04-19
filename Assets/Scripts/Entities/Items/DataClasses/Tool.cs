using UnityEngine;

public class Tool : ItemBase
{
    public Tool(ToolData toolData) : base(toolData)
    {
        ToolType = toolData.ToolType;
    }

    public ToolType ToolType { get; }
}