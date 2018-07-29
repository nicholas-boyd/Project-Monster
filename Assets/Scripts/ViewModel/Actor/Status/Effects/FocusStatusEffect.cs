using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusStatusEffect : StatusEffect
{
    StatModifierFeature feature;
    void OnEnable()
    {
        statusEffectIcon = Resources.Load<Sprite>("Sprites/StatusIcons/FocusStatusIcon");
        feature = gameObject.AddComponent<StatModifierFeature>();
        feature.type = StatTypes.STR;
        feature.amount = GetComponentInParent<Stats>()[StatTypes.STR];
        feature.Activate(GetComponentInParent<Unit>().gameObject);
    }

    void OnDisable()
    {
        feature.Deactivate();
    }
}
