using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManaCost : MonoBehaviour
{
    #region Fields
    public int amount;
    Card owner;
    Mana mana;
    #endregion
    #region MonoBehaviour
    void Awake()
    {
        owner = GetComponent<Card>();
        mana = GetComponentInParent<Unit>().GetComponent<Mana>();
    }

    void OnEnable()
    {
        this.AddObserver(OnCanPerformCheck, Card.AbilityCardCanPerformNotification, owner);
        this.AddObserver(OnDidPerformNotification, Card.AbilityCardActivatedNotification, owner);
    }

    void OnDisable()
    {
        this.RemoveObserver(OnCanPerformCheck, Card.AbilityCardCanPerformNotification, owner);
        this.RemoveObserver(OnDidPerformNotification, Card.AbilityCardActivatedNotification, owner);
    }
    #endregion
    #region Notification Handlers
    void OnCanPerformCheck(object sender, object args)
    {
        if (mana.MP < amount)
        {
            BaseException exc = (BaseException)args;
            exc.FlipToggle();
        }
    }

    void OnDidPerformNotification(object sender, object args)
    {
        mana.MP -= amount;
    }
    #endregion
}
