using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Consumable : Item
{
    public Consumable(ConsumableItemData itemData) : base(itemData)
    {
        IsConsumable = itemData.IsConsumable;
        OnConsumedEffects = itemData.OnConsumedEffects;
    }

    public bool IsConsumable { get; }
    public List<ScriptableStatusEffectData> OnConsumedEffects { get; }
}
