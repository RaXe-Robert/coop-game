﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Items/Consumable")]
public class ConsumableItemData : ScriptableItemData
{
    [SerializeField] private bool isConsumable;
    [SerializeField] private List<ScriptableStatusEffectData> onConsumedEffects;

    public bool IsConsumable { get { return isConsumable; } }
    public List<ScriptableStatusEffectData> OnConsumedEffects { get { return onConsumedEffects; } }

    public override Item InitializeItem()
    {
        return new Consumable(this);
    }
}
