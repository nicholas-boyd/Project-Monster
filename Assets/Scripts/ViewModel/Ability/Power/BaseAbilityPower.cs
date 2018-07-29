using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public abstract class BaseAbilityPower : MonoBehaviour
{
    protected abstract float GetBaseAttack(Unit attacker);
    protected abstract float GetBaseDefense(Unit target);
    public abstract float Power { get; set; }

    void OnEnable()
    {
        this.AddObserver(OnGetBaseAttack, BaseAbilityEffect.GetAttackNotification, GetComponent<BaseAbilityEffect>());
        this.AddObserver(OnGetBaseDefense, BaseAbilityEffect.GetDefenseNotification, GetComponent<BaseAbilityEffect>());
        this.AddObserver(OnGetPower, BaseAbilityEffect.GetPowerNotification, GetComponent<BaseAbilityEffect>());
    }

    void OnDisable()
    {
        this.RemoveObserver(OnGetBaseAttack, BaseAbilityEffect.GetAttackNotification, GetComponent<BaseAbilityEffect>());
        this.RemoveObserver(OnGetBaseDefense, BaseAbilityEffect.GetDefenseNotification, GetComponent<BaseAbilityEffect>());
        this.RemoveObserver(OnGetPower, BaseAbilityEffect.GetPowerNotification, GetComponent<BaseAbilityEffect>());
    }

    void OnGetBaseAttack(object sender, object args)
    {
        var info = (Info<Unit, Unit, List<ValueModifier>>)args;
        if (info.arg0 != GetComponentInParent<Unit>())
            return;
        AddValueModifier mod = new AddValueModifier(0, GetBaseAttack(info.arg0));
        info.arg2.Add(mod);
    }

    void OnGetBaseDefense(object sender, object args)
    {
        var info = (Info<Unit, Unit, List<ValueModifier>>)args;
        if (info.arg0 != GetComponentInParent<Unit>())
            return;

        AddValueModifier mod = new AddValueModifier(0, GetBaseDefense(info.arg1));
        info.arg2.Add(mod);
    }

    void OnGetPower(object sender, object args)
    {
        var info = (Info<Unit, Unit, List<ValueModifier>>)args;
        if (info.arg0 != GetComponentInParent<Unit>())
            return;

        AddValueModifier mod = new AddValueModifier(0, Power);
        info.arg2.Add(mod);
    }
}