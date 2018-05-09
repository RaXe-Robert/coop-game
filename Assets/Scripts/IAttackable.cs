using System;
using UnityEngine;

public interface IAttackable : ITooltip
{
    Vector3 Position { get; }
    void TakeHit(IAttacker attacker);
}

