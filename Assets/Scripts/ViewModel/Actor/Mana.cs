using UnityEngine;
using System.Collections;
public class Mana : MonoBehaviour
{
    #region Fields
    public float MPRecoveryRate = 5;

    public int MP
    {
        get { return Mathf.RoundToInt(stats[StatTypes.MP]); }
        set { stats.SetValue(StatTypes.MP, Mathf.Clamp(value, 0, MMP), false); }
    }

    public int MMP
    {
        get { return Mathf.RoundToInt(stats[StatTypes.MMP]); }
        set { stats.SetValue(StatTypes.MMP, value, false); }
    }

    public bool MPFull
    {
        get { return MP == MMP; }
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
        this.AddObserver(OnMPWillChange, Stats.WillChangeNotification(StatTypes.MP), stats);
        this.AddObserver(OnMMPDidChange, Stats.DidChangeNotification(StatTypes.MMP), stats);
    }

    void OnDisable()
    {
        this.RemoveObserver(OnMPWillChange, Stats.WillChangeNotification(StatTypes.MP), stats);
        this.RemoveObserver(OnMMPDidChange, Stats.DidChangeNotification(StatTypes.MMP), stats);
    }
    #endregion

    #region Event Handlers
    void OnMPWillChange(object sender, object args)
    {
        ((ValueChangeException)args).AddModifier(new ClampValueModifier(int.MaxValue, 0, int.MaxValue));

    }

    void OnMMPDidChange(object sender, object args)
    {
        int oldMMP = Mathf.RoundToInt((float)args);
        if (MMP > oldMMP)
            MP += MMP - oldMMP;
        else
            MP = Mathf.Clamp(MP, 0, MMP);
    }
    #endregion

    #region Private
    private void Update()
    {
        if (!MPFull)
        {
            stats.SetValue(StatTypes.MP, Mathf.Clamp(stats[StatTypes.MP] + (MPRecoveryRate*Time.deltaTime), 0, MMP), false);
        }
    }
    #endregion
}