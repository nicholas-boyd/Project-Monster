using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RandomCardPicker : BaseCardPicker
{
    public List<BaseCardPicker> pickers;
    public override void Pick(PlanOfAttack plan)
    {
        int index = Random.Range(0, pickers.Count);
        BaseCardPicker p = pickers[index];
        p.Pick(plan);
    }
}