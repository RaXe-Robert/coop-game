using System;
using UnityEngine;

public interface IAttackable : ITooltip
{
    GameObject GameObject { get; }
    void TakeHit(IAttacker attacker);
}

