using System.Collections.Generic;
using UnityEngine;

public class ConsumableItemData : ScriptableItemData
{
    [SerializeField] private bool isConsumable;
    [SerializeField] private List<ScriptableStatusEffectData> onConsumedEffects;

    public bool IsConsumable { get { return isConsumable; } }
    public List<ScriptableStatusEffectData> OnConsumedEffects { get { return onConsumedEffects; } }
}
