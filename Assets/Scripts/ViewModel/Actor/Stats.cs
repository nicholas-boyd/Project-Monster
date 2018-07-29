using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Stats : MonoBehaviour
{
    public float this[StatTypes s]
    {
        get { return _data[(int)s]; }
        set { SetValue(s, value, true); }
    }
    float[] _data = new float[(int)StatTypes.Count];

    public const string UnitUpdateNotification = "UNIT_UPDATE";

    public static string WillChangeNotification(StatTypes type)
    {
        if (!_willChangeNotifications.ContainsKey(type))
            _willChangeNotifications.Add(type, string.Format("Stats.{0}WillChange", type.ToString()));
        return _willChangeNotifications[type];
    }

    public static string DidChangeNotification(StatTypes type)
    {
        if (!_didChangeNotifications.ContainsKey(type))
            _didChangeNotifications.Add(type, string.Format("Stats.{0}DidChange", type.ToString()));
        return _didChangeNotifications[type];
    }

    static Dictionary<StatTypes, string> _willChangeNotifications = new Dictionary<StatTypes, string>();
    static Dictionary<StatTypes, string> _didChangeNotifications = new Dictionary<StatTypes, string>();

    public float SetValue(StatTypes type, float value, bool allowExceptions)
    {
        float oldValue = this[type];

        if (allowExceptions)
        {
            // Allow exceptions to the rule here
            ValueChangeException exc = new ValueChangeException(oldValue, value);

            // The notification is unique per stat type
            this.PostNotification(WillChangeNotification(type), exc);

            // Did anything modify the value?
            value = Mathf.FloorToInt(exc.GetModifiedValue());

            // Did something nullify the change?
            if (exc.toggle == false || value == oldValue)
                return oldValue;
        }


        if (oldValue == value)
            return oldValue;

        // Modify value for special cases (ie Health/Mana will not go lower than 0, or higher than MHP/MMP)
        switch (type)
        {
            case StatTypes.HP:
                value = Mathf.Clamp(value, 0, this[StatTypes.MHP]);
                break;
            case StatTypes.MP:
                value = Mathf.Clamp(value, 0, this[StatTypes.MMP]);
                break;
        }

        // Set Value
        _data[(int)type] = value;

        // Wrap up other stat changes affected by value, and send completed notifications
        if (type == StatTypes.DEX)
        {
            if (_data[(int)type] <= 0)
                _data[(int)StatTypes.SPD] = 0;
            else
                _data[(int)StatTypes.SPD] = 1 / _data[(int)type] / 2;
        }
            
            
        this.PostNotification(DidChangeNotification(type), oldValue);
        this.PostNotification(UnitUpdateNotification, GetComponent<Unit>());
        return value;
    }

    public void AddValue(StatTypes type, float value, bool allowExceptions)
    {
        float oldValue = this[type];
        SetValue(type, Mathf.Clamp(oldValue+value, 0, int.MaxValue), allowExceptions);
    }
}