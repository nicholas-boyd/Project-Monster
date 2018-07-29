using UnityEngine;
using System.Collections;
public class AllyAbilityEffectTarget : AbilityEffectTarget
{
    public override int maxTargets()
    {
        return 1;
    }

    public override bool IsTarget(Tile tile)
    {
        if (tile == null || tile.content == null)
            return false;
        return tile.content.GetComponent<Alliance>().type == GetComponentInParent<Alliance>().type;
    }
}