using UnityEngine;
using System.Collections;
public class Job : MonoBehaviour
{
    #region Fields / Properties
    public static readonly StatTypes[] statOrder = new StatTypes[] {
        StatTypes.MHP,
        StatTypes.MMP,
        StatTypes.STR,
        StatTypes.CHA,
        StatTypes.CON,
        StatTypes.WIS,
        StatTypes.DEX
    };
    public int[] baseStats = new int[statOrder.Length];
    Stats stats;
    #endregion
    #region Public
    public void Employ()
    {
        stats = gameObject.GetComponentInParent<Stats>();
        LoadDefaultStats();
        Feature[] features = GetComponentsInChildren<Feature>();
        for (int i = 0; i < features.Length; ++i)
            features[i].Activate(gameObject);
    }

    public void UnEmploy()
    {
        Feature[] features = GetComponentsInChildren<Feature>();
        for (int i = 0; i < features.Length; ++i)
            features[i].Deactivate();
        stats = null;
    }

    public void LoadDefaultStats()
    {
        for (int i = 0; i < statOrder.Length; ++i)
        {
            StatTypes type = statOrder[i];
            if (type == StatTypes.DEX)
                Debug.Log("Setting default DEX " + baseStats[i]);
            stats.SetValue(type, baseStats[i], false);
        }
        stats.SetValue(StatTypes.HP, stats[StatTypes.MHP], false);
        stats.SetValue(StatTypes.MP, stats[StatTypes.MMP], false);
    }
    #endregion
}