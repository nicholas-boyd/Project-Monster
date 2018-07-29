using UnityEngine;
using System.Collections;
public class AlliesAbilityEffectTarget : AllyAbilityEffectTarget
{
    public override int maxTargets()
    {
        return int.MaxValue;
    }
}