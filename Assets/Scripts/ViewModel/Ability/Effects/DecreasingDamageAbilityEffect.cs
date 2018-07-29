using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DecreasingDamageAbilityEffect : DamageAbilityEffect
{

    #region Public
    void OnEnable()
    {
        this.AddObserver(OnGetPower, BaseAbilityEffect.GetPowerNotification, GetComponent<BaseAbilityEffect>());
    }

    void OnDisable()
    {
        this.RemoveObserver(OnGetPower, BaseAbilityEffect.GetPowerNotification, GetComponent<BaseAbilityEffect>());
    }

    void OnGetPower(object sender, object args)
    {
        var info = (Info<Unit, Unit, List<ValueModifier>>)args;
        if (info.arg0 != GetComponentInParent<Unit>())
            return;

        AddValueModifier mod = new AddValueModifier(0, -GetDistance(info.arg0.tile, info.arg1.tile)*20);
        info.arg2.Add(mod);
    }

    protected int GetDistance(Tile from, Tile to)
    {
        return Mathf.Abs(from.pos.x - to.pos.x) + Mathf.Abs(from.pos.y - to.pos.y);
    }
    #endregion
}