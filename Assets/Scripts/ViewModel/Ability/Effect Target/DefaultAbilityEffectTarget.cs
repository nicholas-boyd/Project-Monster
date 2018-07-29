using UnityEngine;
using System.Collections;
public class DefaultAbilityEffectTarget : AbilityEffectTarget {
    public override int maxTargets() {
        return int.MaxValue;
    }

	public override bool IsTarget (Tile tile) {
		if (tile == null || tile.content == null)
			return false;
		return true;
	}
}