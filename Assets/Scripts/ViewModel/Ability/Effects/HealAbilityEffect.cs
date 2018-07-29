using UnityEngine;
using System.Collections;

public class HealAbilityEffect : BaseAbilityEffect
{
    public const string UnitHealedNotification = "UNIT_HEALED";

    public override int Predict(Tile target)
    {
        Unit attacker = GetComponentInParent<Unit>();
        Unit defender = target.content.GetComponent<Unit>();
        return GetStat(attacker, defender, GetPowerNotification, 0);
    }

    protected override int Apply(Tile target)
    {
        Unit attacker = GetComponentInParent<Unit>();
        Unit defender = target.content.GetComponent<Unit>();

        // Start with the predicted value
        int value = Predict(target);
        

        // Clamp the amount to a range
        value = Mathf.Clamp(value, minDamage, maxDamage);

        // Apply the amount to the target
        Stats s = defender.GetComponent<Stats>();
        float oldHP = s[StatTypes.HP];
        float newHP = s.SetValue(StatTypes.HP, Mathf.Clamp(s[StatTypes.HP]+value, 1, s[StatTypes.MHP]), false);
        Info<Unit, Unit, int> info = new Info<Unit, Unit, int>(attacker, defender, Mathf.RoundToInt(newHP-oldHP));
        defender.beingHealed = false;
        this.PostNotification(UnitHealedNotification, info);
        return value;
    }
}