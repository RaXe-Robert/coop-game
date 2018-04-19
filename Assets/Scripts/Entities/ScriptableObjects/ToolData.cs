using UnityEngine;

public enum ToolType { Axe, Pickaxe }

[CreateAssetMenu(fileName = "New Tool", menuName = "Entities/Items/Tool")]
public class ToolData : ScriptableEntityData
{
    [SerializeField] private ToolType toolType;
    public ToolType ToolType { get { return toolType; } }

    public override EntityBase InitializeEntity()
    {
        return new Tool(this);
    }
}


