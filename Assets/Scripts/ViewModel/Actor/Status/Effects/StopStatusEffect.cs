using UnityEngine;
using System.Collections;
public class StopStatusEffect : StatusEffect
{
    StatModifierFeature feature;
    void OnEnable()
    {
        statusEffectIcon = Resources.Load<Sprite>("Sprites/StatusIcons/StopStatusIcon");
        feature = gameObject.AddComponent<StatModifierFeature>();
        feature.type = StatTypes.DEX;
        feature.amount = -1000;
        feature.Activate(GetComponentInParent<Unit>().gameObject);
    }

    void OnDisable()
    {
        feature.Deactivate();
    }
}
