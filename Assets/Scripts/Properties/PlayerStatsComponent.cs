using UnityEngine;
using System.Collections;

public class PlayerStatsComponent : MonoBehaviour
{
    [SerializeField] private BaseStatsData baseStats;

    public delegate void OnValueChanged();
    public OnValueChanged OnValueChangedCallback;

    //Stats
    private Stat movementSpeed;
    private Stat minDamage;
    private Stat maxDamage;
    private Stat timeBetweenAttacks;
    private Stat timeBetweenResourceHits;
    private Stat defense;
    private EquipmentManager equipmentManager;

    public float MovementSpeed => movementSpeed.CurrentValue;
    public float MinDamage => minDamage.CurrentValue;
    public float MaxDamage => maxDamage.CurrentValue;
    public float TimeBetweenAttacks => timeBetweenAttacks.CurrentValue;
    public float TimeBetweenResourceHits => timeBetweenResourceHits.CurrentValue;
    public float Defense => defense.CurrentValue;
    public float Damage => Random.Range(minDamage.CurrentValue, maxDamage.CurrentValue);

    private void Awake()
    {
        equipmentManager = GetComponent<EquipmentManager>();

        equipmentManager.OnArmorEquippedCallback += ApplyArmorStats;
        equipmentManager.OnArmorUnequippedCallback += RemoveArmorStats;

        InitializeStats();
    }

    private void InitializeStats()
    {
        movementSpeed = new Stat(baseStats.movementSpeed, this);
        minDamage = new Stat(baseStats.minDamage, this);
        maxDamage = new Stat(baseStats.maxDamage, this);
        timeBetweenAttacks = new Stat(baseStats.timeBetweenAttacks, this);
        timeBetweenResourceHits = new Stat(baseStats.timeBetweenResourceHits, this);
        defense = new Stat(baseStats.defense, this);
    }

    private void ApplyArmorStats(Armor armor)
    {
        defense.AddValue(armor.Defense);
    }

    private void RemoveArmorStats(Armor armor)
    {
        defense.RemoveValue(armor.Defense);
    }
}
