using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAbilityPower : BaseAbilityPower
{
    public override float Power { get; set; }

    protected override float GetBaseAttack(Unit attacker)
    {
        return attacker.GetComponent<Stats>()[StatTypes.WIS];
    }

    protected override float GetBaseDefense(Unit target)
    {
        return target.GetComponent<Stats>()[StatTypes.CHA];
    }
}
