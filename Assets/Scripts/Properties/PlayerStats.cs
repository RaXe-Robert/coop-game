using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private BaseStatsData baseStats;

    public delegate void OnValueChanged();
    public OnValueChanged OnValueChangedCallback;

    //Stats
    private Stat movementSpeed;
    private Stat minDamage;
    private Stat maxDamage;
    private Stat attackPerSecond;
    private Stat defense;

    public float MovementSpeed => movementSpeed.CurrentValue;
    public float MinDamage => minDamage.CurrentValue;
    public float MaxDamage => maxDamage.CurrentValue;
    public float AttacksPerSecond => attackPerSecond.CurrentValue;
    public float Defense => defense.CurrentValue;

    private EquipmentManager equipmentManager;

    private void Awake()
    {
        equipmentManager = GetComponent<EquipmentManager>();

        equipmentManager.OnWeaponEquippedCallback += EquipWeapon;
        equipmentManager.OnWeaponUnequippedCallback += UnEquipWeapon;
        equipmentManager.OnArmorEquippedCallback += EquipArmor;
        equipmentManager.OnArmorUnequippedCallback += UnEquipArmor;

        InitializeStats();
    }

    private void Update()
    {
        //Testing
        if (Input.GetKeyDown(KeyCode.Alpha1))
            movementSpeed.AddValue(MovementSpeed / 10);
    }

    private void InitializeStats()
    {
        movementSpeed = new Stat(baseStats.movementSpeed, this);
        minDamage = new Stat(baseStats.minDamage, this);
        maxDamage = new Stat(baseStats.maxDamage, this);
        attackPerSecond = new Stat(baseStats.attacksPerSecond, this);
        defense = new Stat(baseStats.defense, this);
    }

    private void EquipWeapon(Weapon weapon)
    {
        minDamage.AddValue(weapon.MinDamage);
        maxDamage.AddValue(weapon.MaxDamage);
        attackPerSecond.AddValue(weapon.AttacksPerSecond - baseStats.attacksPerSecond);
    }

    private void UnEquipWeapon(Weapon weapon)
    {
        minDamage.RemoveValue(weapon.MinDamage);
        maxDamage.RemoveValue(weapon.MaxDamage);
        attackPerSecond.RemoveValue(weapon.AttacksPerSecond - baseStats.attacksPerSecond);
    }

    private void EquipArmor(Armor armor)
    {
        defense.AddValue(armor.Defense);
    }

    private void UnEquipArmor(Armor armor)
    {
        defense.RemoveValue(armor.Defense);
    }
}
