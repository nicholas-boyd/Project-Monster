using UnityEngine;
using System.Collections;
public class FoeAbilityEffectTarget : AbilityEffectTarget
{
    public override int maxTargets()
    {
        return 1;
    }

    public override bool IsTarget(Tile tile)
    {
        if (tile == null || tile.content == null)
            return false;
        Alliances me = GetComponentInParent<Alliance>().type;
        Alliances target = tile.content.GetComponent<Alliance>().type;
        switch (me)
        {
            case Alliances.Enemy:
                return target != Alliances.Enemy;
            case Alliances.Hero:
                return target != Alliances.Hero;
            case Alliances.Neutral:
                return false;
            case Alliances.None:
            default:
                return true;
        }
    }
}