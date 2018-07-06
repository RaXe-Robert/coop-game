using UnityEngine;

public enum ToolType { Nothing, Axe, Pickaxe, Hammer }

[CreateAssetMenu(fileName = "New Tool", menuName = "Items/Tool")]
public class ToolData : ScriptableItemData
{
    [SerializeField] private ToolType toolType;
    public ToolType ToolType { get { return toolType; } }

    public override Item InitializeItem()
    {
        return new Tool(this);
    }
}


