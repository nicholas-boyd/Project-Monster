using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public const string AbilityCardCanPerformNotification = "ABILITYCARD_CAN_PERFORM";
    public const string AbilityCardCantPerformNotification = "ABILITYCARD_CANT_PERFORM";
    public const string AbilityCardActivatedNotification = "ABILITYCARD_ACTIVATED";
    string Name;
    string Desc;
    CardRecipe data;
    ParticleSystem visualEffect;
    Tweener projectileTweener;
    public float ID;

    public void Load(CardRecipe data)
    {
        AbilityManaCost abilityManaCost = GetComponent<AbilityManaCost>();
        if (abilityManaCost == null)
        {
            abilityManaCost = gameObject.AddComponent<AbilityManaCost>();
        }

        AbilityEffectTarget abilityEffectTarget = GetComponent<AbilityEffectTarget>();
        if (abilityEffectTarget != null)
        {
            DestroyImmediate(abilityEffectTarget, true);
        }

        BaseAbilityEffect abilityEffect = GetComponent<BaseAbilityEffect>();
        if (abilityEffect != null)
        {
            DestroyImmediate(abilityEffect, true);
        }

        BaseAbilityPower power = GetComponent<BaseAbilityPower>();
        if (power != null)
        {
            DestroyImmediate(power, true);
        }

        AbilityRange range = GetComponent<AbilityRange>();
        if (range != null)
        {
            DestroyImmediate(range, true);
        }

        if (data == null)
        {
            Name = "";
            Desc = "";
            abilityManaCost.amount = 0;
            abilityEffectTarget = gameObject.AddComponent<DefaultAbilityEffectTarget>();
            abilityEffect = gameObject.AddComponent<DamageAbilityEffect>();
            power = gameObject.AddComponent<PhysicalAbilityPower>();
            power.Power = 0;
            range = gameObject.AddComponent<ConstantAbilityRange>();
            range.horizontal = 0;
            this.data = Resources.Load("Recipes/CardRecipes/Empty Card Data") as CardRecipe;
            ID = GetInstanceID();
            visualEffect = null;
            return;
        }
        Name = data.Name;
        Desc = data.Description;
        abilityManaCost.amount = data.AbilityManaCost;
        abilityEffectTarget = gameObject.AddComponent(Type.GetType(data.AbilityEffectTarget)) as AbilityEffectTarget;

        if (data.AbilityEffect.Contains("InflictStatus"))
        {
            string ae = "InflictStatusAbilityEffect";
            string status = string.Format("{0}StatusEffect", data.AbilityEffect.Substring(13, data.AbilityEffect.Length - 26));

            InflictStatusAbilityEffect effect = gameObject.AddComponent(Type.GetType(ae)) as InflictStatusAbilityEffect;
            effect.statusName = status;
            abilityEffect = effect;
        }
        else
        {
            abilityEffect = gameObject.AddComponent(Type.GetType(data.AbilityEffect)) as BaseAbilityEffect;
        }

        power = gameObject.AddComponent(Type.GetType(data.AbilityPower)) as BaseAbilityPower;
        power.Power = data.BasePower;
        range = gameObject.AddComponent(Type.GetType(data.AbilityRange)) as AbilityRange;
        range.horizontal = data.AbilityRangeHorizontal;
        this.data = data;
        ID = GetInstanceID();
        visualEffect = data.ParticleEffect;
    }

    public bool CanPerform()
    {
        BaseException exc = new BaseException(true);
        this.PostNotification(AbilityCardCanPerformNotification, exc);
        return exc.toggle;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetDesc()
    {
        return Desc;
    }

    public CardRecipe GetData()
    {
        return data;
    }

    public void Perform(List<Tile> targets)
    {
        Unit parent = GetComponentInParent<Unit>();
        int limit = GetComponent<AbilityEffectTarget>().maxTargets();
        if (!CanPerform())
        {
            if (parent.name == "Hero")
                this.PostNotification(AbilityCardCantPerformNotification);
            return;
        }
        Info<Unit, Card> info = new Info<Unit, Card>(parent, this);
        this.PostNotification(AbilityCardActivatedNotification, info);
        if (visualEffect != null && (data.ParticalEffectType == ParticleEffectType.Single || data.ParticalEffectType == ParticleEffectType.Hybrid))
        {
            ParticleSystem marker = parent.tile.pos.x < 3 ? Instantiate(data.RedHitMarker, parent.tile.center, Quaternion.identity) : Instantiate(data.BlueHitMarker, parent.tile.center, Quaternion.identity);
            PerformParticleEffect(parent, parent.tile);
        }

        for (int i = 0; (i < targets.Count) && (i < limit); ++i)
        {
            if (visualEffect != null && (data.ParticalEffectType == ParticleEffectType.Multiple || data.ParticalEffectType == ParticleEffectType.Hybrid))
            {
                ParticleSystem marker = targets[i].pos.x < 3 ? Instantiate(data.RedHitMarker, targets[i].center, Quaternion.identity) : Instantiate(data.BlueHitMarker, targets[i].center, Quaternion.identity);
                PerformParticleEffect(parent, targets[i]);
            }

            if (targets[i].content != null)
            {
                Perform(targets[i]);
            }
        }
        
    }

    void Perform(Tile target)
    {
        BaseAbilityEffect effect = transform.GetComponentInChildren<BaseAbilityEffect>();
        effect.OnApply(target);
    }

    void PerformParticleEffect(Unit attacker, Tile target)
    {
        ParticleSystem visual = Instantiate(visualEffect);
        if (data.ProjectileEffect)
        {
            visual.transform.position = attacker.tile.center + new Vector3(attacker.tile.GetComponent<BoxCollider>().bounds.size.x / 2, attacker.GetComponent<SpriteRenderer>().bounds.size.y / 2, 0);
            StartCoroutine(MoveProjectileEffect(visual, target.center));
        }
        else
        {
            visual.transform.position = target.center;
        }

    }

    IEnumerator MoveProjectileEffect(ParticleSystem obj, Vector3 pos)
    {
        projectileTweener = TransformAnimationExtensions.MoveTo(obj.transform, pos);
        projectileTweener.easingControl.duration = 1f;
        projectileTweener.easingControl.equation = EasingEquations.EaseOutCubic;
        while (projectileTweener != null)
            yield return null;
    }
}
