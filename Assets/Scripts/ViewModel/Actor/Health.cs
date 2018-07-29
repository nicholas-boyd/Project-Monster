using UnityEngine;
using System.Collections;
using System;

public class Health : MonoBehaviour
{
    #region Fields
    public int HP
    {
        get { return Mathf.RoundToInt(stats[StatTypes.HP]); }
        set { stats.SetValue(StatTypes.HP, Mathf.Clamp(value, 0, stats[StatTypes.MHP]), false); }
    }

    public int MHP
    {
        get { return Mathf.RoundToInt(stats[StatTypes.MHP]); }
        set { stats.SetValue(StatTypes.MHP, value, false); }
    }

    public bool FullHP
    {
        get { return HP == MHP; }
    }
    Stats stats;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        stats = GetComponent<Stats>();
    }

    void OnEnable()
    {
        this.AddObserver(OnHPWillChange, Stats.WillChangeNotification(StatTypes.HP), stats);
        this.AddObserver(OnMHPDidChange, Stats.DidChangeNotification(StatTypes.MHP), stats);
    }

    void OnDisable()
    {
        this.RemoveObserver(OnHPWillChange, Stats.WillChangeNotification(StatTypes.HP), stats);
        this.RemoveObserver(OnMHPDidChange, Stats.DidChangeNotification(StatTypes.MHP), stats);
    }
    #endregion
    #region Event Handlers
    void OnHPWillChange(object sender, object args)
    {
        ((ValueChangeException)args).AddModifier(new ClampValueModifier(int.MaxValue, 0, stats[StatTypes.MHP]));
    }

    void OnMHPDidChange(object sender, object args)
    {
        int oldMHP = Mathf.RoundToInt((float)args);
        if (MHP > oldMHP)
            HP += MHP - oldMHP;
        else
            HP = Mathf.Clamp(HP, 0, MHP);
    }
    #endregion
}