using System;
using UnityEngine;

public interface IAttackable
{
    Vector3 Position { get; }
    void TakeHit(IAttacker attacker);
}

