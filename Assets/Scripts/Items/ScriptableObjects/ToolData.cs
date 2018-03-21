using UnityEngine;

public enum ToolType { Axe, Pickaxe }

[CreateAssetMenu(fileName = "New Tool", menuName = "Items/Tool")]
public class ToolData : ScriptableItemData
{
    public override ItemBase InitializeItem()
    {
        return new Tool(this);
    }
}


