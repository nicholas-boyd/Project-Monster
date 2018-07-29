using UnityEngine;
using System.Collections;
public class SlowStatusEffect : StatusEffect
{
    StatModifierFeature feature;
    void OnEnable()
    {
        statusEffectIcon = Resources.Load<Sprite>("Sprites/StatusIcons/SlowStatusIcon");
        feature = gameObject.AddComponent<StatModifierFeature>();
        feature.type = StatTypes.DEX;
        feature.amount = -GetComponentInParent<Stats>()[StatTypes.DEX]/2;
        feature.Activate(GetComponentInParent<Unit>().gameObject);
    }

    void OnDisable()
    {
        feature.Deactivate();
    }
}