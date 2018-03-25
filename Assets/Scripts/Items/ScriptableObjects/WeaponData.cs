﻿using UnityEngine;

public enum WeaponType { Sword, Axe, Mace, Bow }

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
public class WeaponData : ScriptableItemData
{
    [SerializeField] private WeaponType type;
    [SerializeField] private float minDamage;
    [SerializeField] private float maxDamage;
    [SerializeField] private float attacksPerSecond;

    public WeaponType Type { get { return type; } }
    public float MinDamage { get { return minDamage; } }
    public float MaxDamage { get { return maxDamage; } }
    public float AttacksPerSecond { get { return attacksPerSecond; } }

    public override ItemBase InitializeItem()
    {
        return new Weapon(this);
    }
}
