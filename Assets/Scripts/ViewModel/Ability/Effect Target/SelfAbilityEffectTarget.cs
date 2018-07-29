using UnityEngine;
using System.Collections;
public class SelfAbilityEffectTarget : AbilityEffectTarget
{
    public override int maxTargets()
    {
        return 1;
    }

    public override bool IsTarget(Tile tile)
    {
        if (tile == null || tile.content == null)
            return false;
        return tile.content.GetComponent<Unit>() == GetComponentInParent<Unit>();
    }
}