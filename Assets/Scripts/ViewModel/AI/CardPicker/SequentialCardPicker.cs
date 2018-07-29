using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SequentialCardPicker : BaseCardPicker
{
    public List<BaseCardPicker> pickers;
    public override void Pick(PlanOfAttack plan)
    {
        for (int i = 0; i < pickers.Count; i++)
        {
            BaseCardPicker p = pickers[i];
            p.Pick(plan);
            if (plan.ability != null)
                break;
        }
    }
}