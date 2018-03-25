using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] BaseStats baseStats;

    //Stats
    private Stat maxHealth;
    private Stat currentHealth;
    private Stat movementSpeed;
    private Stat minDamage;
    private Stat maxDamage;

    private EquipmentManager equipmentManager;

    void Start()
    {



        equipmentManager = GetComponent<EquipmentManager>();

        equipmentManager.OnWeaponEquippedCallback += EquipWeapon;
    }

    void Update()
    {

    }

    private void EquipWeapon(Weapon weapon)
    {
        baseMinDamage = weapon.MinDamage;
        baseMaxDamage = weapon.MaxDamage;
        baseAttacksPerSecond = weapon.AttacksPerSecond;
    }
}

public class Stat
{
    private float baseValue;
    private float currentValue;

    public Stat(float baseValue)
    {
        currentValue = baseValue;
    }

    public void AddValue(float value)
    {
        currentValue += value;
    }

    public void RemoveValue(float value)
    {
        currentValue -= value;
    }
}

public class BaseStats : ScriptableObject
{
    public float maxHealth;
    public float movementSpeed;
    public float minDamage;
    public float maxDamage;
    public float attacksPerSecond;
}
