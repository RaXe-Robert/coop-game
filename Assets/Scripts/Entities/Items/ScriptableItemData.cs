using System.Collections.Generic;

using UnityEngine;

public abstract class ScriptableItemData : ScriptableEntityData
{
    [SerializeField] private bool isConsumable;
    [SerializeField] private List<ScriptableStatusEffectData> onConsumedEffects;

    public bool IsConsumable { get { return isConsumable; } }
    public List<ScriptableStatusEffectData> OnConsumedEffects { get { return onConsumedEffects; } }
}
