using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class ItemBase : EntityBase
{
    public ItemBase(ScriptableItemData itemData) : base(itemData)
    {
        IsConsumable = itemData.IsConsumable;
        OnConsumedEffects = itemData.OnConsumedEffects;
    }

    public bool IsConsumable { get; }
    public List<ScriptableStatusEffectData> OnConsumedEffects { get; }

    public bool Equippable { get { return GetType() == typeof(Armor) || GetType() == typeof(Tool) || GetType() == typeof(Weapon); } }
}
