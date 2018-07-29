using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmartHealCardPicker : BaseCardPicker
{
    public Targets target;
    public string card;
    public override void Pick(PlanOfAttack plan)
    {
        plan.ability = Find(card);
        plan.target = target;
        bool noNeedToHeal = true;
        if (plan.ability != null)
        {
            List<Tile> targets = new List<Tile>(GetComponentInParent<BattleController>().board.tiles.Values);
            foreach(Tile t in targets)
            {
                if(t.content != null && t.content.GetComponent<Alliance>().IsMatch(owner.GetComponent<Alliance>(), target) && plan.ability.GetComponent<AbilityEffectTarget>().IsTarget(t))
                {
                    if (!t.content.GetComponent<Health>().FullHP || t.content.GetComponent<Unit>().beingHealed)
                        noNeedToHeal = false;
                }
            }
        }

        if (noNeedToHeal)
        {
            plan.ability = null;
        }
    }
}