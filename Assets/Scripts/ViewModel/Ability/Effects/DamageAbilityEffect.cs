using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageAbilityEffect : BaseAbilityEffect
{
    #region Consts
    public const string DamageDealtNotification = "DAMAGE_DEALT";
    #endregion

    #region Public
    public override int Predict(Tile target)
    {
        Unit attacker = GetComponentInParent<Unit>();
        Unit defender = target.content.GetComponent<Unit>();

        // Get the attackers base attack stat considering
        // mission items, support check, status check, and equipment, etc
        int attack = GetStat(attacker, defender, GetAttackNotification, 0);


        // NOTE: Defense is currently not implemented
        // Get the targets base defense stat considering
        // mission items, support check, status check, and equipment, etc
        // int defense = GetStat(attacker, defender, GetDefenseNotification, 0);

        // Get the abilities power stat considering possible variations
        int power = GetStat(attacker, defender, GetPowerNotification, 0);

        // Calculate base damage
        int damage = power + attack;

        // Tweak the damage based on a variety of other checks like
        // Elemental damage, Critical Hits, Damage multipliers, etc.
        damage = GetStat(attacker, defender, TweakDamageNotification, damage);

        // Clamp the damage to a range
        damage = Mathf.Clamp(damage, minDamage, maxDamage);
        Info<Unit, Unit, int> info = new Info<Unit, Unit, int>(attacker, defender, damage);
        this.PostNotification(DamageDealtNotification, info);
        return -damage;
    }

    protected override int Apply(Tile target)
    {
        Unit defender = target.content.GetComponent<Unit>();

        // Start with the predicted damage value
        int value = Predict(target);

        // Add some random variance
        //value = Mathf.FloorToInt(value * UnityEngine.Random.Range(0.9f, 1.1f));

        // Clamp the damage to a range
        value = Mathf.Clamp(value, minDamage, maxDamage);

        // Apply the damage to the target
        Stats s = defender.GetComponent<Stats>();
        s.SetValue(StatTypes.HP, s[StatTypes.HP]+value, false);
        return value;
    }
    #endregion
}