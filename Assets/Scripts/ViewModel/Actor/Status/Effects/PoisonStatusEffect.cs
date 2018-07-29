using UnityEngine;
using System.Collections;
public class PoisonStatusEffect : StatusEffect
{
    Unit owner;
    float tick;

    void Start()
    {
        tick = 1;
        statusEffectIcon = Resources.Load<Sprite>("Sprites/StatusIcons/PoisonStatusIcon");
    }

    void Update() {
        tick = Mathf.Clamp(tick - Time.deltaTime, 0, int.MaxValue);
        if (tick == 0)
        {
            OnSecond();
            tick = 1;
        }

    }

    void OnSecond() {
        Stats s = GetComponentInParent<Stats>();
        int currentHP = Mathf.RoundToInt(s[StatTypes.HP]);
        int maxHP = Mathf.RoundToInt(s[StatTypes.MHP]);
        int reduce = Mathf.Min(currentHP, Mathf.FloorToInt(maxHP * 0.1f));
        s.SetValue(StatTypes.HP, (currentHP - reduce), false);
    }
}