using UnityEngine;
using System.Collections;
public class FixedCardPicker : BaseCardPicker
{
    public Targets target;
    public string card;
    public override void Pick(PlanOfAttack plan)
    {
        plan.ability = Find(card);
        plan.target = target;
    }
}