using UnityEngine;
using System.Collections;
public class HasteStatusEffect : StatusEffect
{
    StatModifierFeature feature;
    void OnEnable()
    {
        statusEffectIcon = Resources.Load<Sprite>("Sprites/StatusIcons/HasteStatusIcon");
        feature = gameObject.AddComponent<StatModifierFeature>();
        feature.type = StatTypes.DEX;
        feature.amount = GetComponentInParent<Stats>()[StatTypes.DEX];
        feature.Activate(GetComponentInParent<Unit>().gameObject);
    }

    void OnDisable()
    {
        feature.Deactivate();
    }
}
