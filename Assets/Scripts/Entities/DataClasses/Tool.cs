using UnityEngine;

public class Tool : EntityBase
{
    public Tool(ToolData toolData) : base(toolData)
    {
        ToolType = toolData.ToolType;
    }

    public ToolType ToolType { get; }
}