using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public class InflictStatusAbilityEffect : BaseAbilityEffect
{
    public string statusName;

    public override int Predict(Tile target)
    {
        Unit attacker = GetComponentInParent<Unit>();
        Unit defender = target.content.GetComponent<Unit>();
        
        // Get the attackers base attack stat considering
        // mission items, support check, status check, and equipment, etc
        int attack = GetStat(attacker, defender, GetAttackNotification, 0);

        // Get the targets base defense stat considering
        // mission items, support check, status check, and equipment, etc
        int defense = GetStat(attacker, defender, GetDefenseNotification, 0);

        // Get the abilities power stat considering possible variations
        int power = GetStat(attacker, defender, GetPowerNotification, 0);

        // Calculate base damage
        int duration = power + attack;
        if (attacker != defender)
            duration -= defense;
        else
            duration += defense;

        // Tweak the damage based on a variety of other checks like
        // Elemental damage, Critical Hits, Damage multipliers, etc.
        duration = GetStat(attacker, defender, TweakDamageNotification, duration);

        // Clamp the damage to a range
        duration = Mathf.Clamp(duration, minDamage, maxDamage);
        return duration;
    }

    protected override int Apply(Tile target)
    {
        Type statusType = Type.GetType(statusName);
        if (statusType == null || !statusType.IsSubclassOf(typeof(StatusEffect)))
        {
            Debug.LogError("Invalid Status Type");
            return 0;
        }

        MethodInfo mi = typeof(Status).GetMethod("Add");
        Type[] types = new Type[] { statusType, typeof(DurationStatusCondition) };
        MethodInfo constructed = mi.MakeGenericMethod(types);

        Status status = target.content.GetComponent<Status>();
        object retValue = constructed.Invoke(status, null);

        DurationStatusCondition condition = retValue as DurationStatusCondition;
        condition.duration = Predict(target);
        return Mathf.RoundToInt(condition.duration);
    }

    public void SetStatusEffectName(string newName)
    {
        statusName = newName;
    }
}