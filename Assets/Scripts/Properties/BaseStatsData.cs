﻿using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New BaseStats", menuName = "Stats/Player")]
public class BaseStatsData : ScriptableObject
{
    public float maxHealth;
    public float movementSpeed;
    public float minDamage;
    public float maxDamage;
    public float timeBetweenAttacks;
    public float defense;
}
