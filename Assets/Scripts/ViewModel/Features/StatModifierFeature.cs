using UnityEngine;
using System.Collections;
public class StatModifierFeature : Feature
{
    #region Fields / Properties
    public StatTypes type;
    public float amount;
    Stats stats
    {
        get
        {
            return _target.GetComponentInParent<Stats>();
        }
    }
    #endregion
    #region Protected
    protected override void OnApply()
    {
        stats.AddValue(type, amount, false);
    }

    protected override void OnRemove()
    {
        stats.AddValue(type, -amount, false);
    }
    #endregion
}