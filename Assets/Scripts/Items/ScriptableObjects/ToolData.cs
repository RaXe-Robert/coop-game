﻿using UnityEngine;

public enum ToolType { Axe, Pickaxe }

[CreateAssetMenu(fileName = "New Tool", menuName = "Items/Tool")]
public class ToolData : ScriptableItemData
{
    [SerializeField] private ToolType toolType;
    public ToolType ToolType { get { return toolType; } }

    public override ItemBase InitializeItem()
    {
        return new Tool(this);
    }
}


