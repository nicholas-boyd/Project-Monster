using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoesAbilityEffectTarget : FoeAbilityEffectTarget
{
    public override int maxTargets()
    {
        return int.MaxValue;
    }
}
