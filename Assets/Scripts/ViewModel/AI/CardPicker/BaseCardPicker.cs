using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCardPicker : MonoBehaviour {

    #region Fields
    protected Unit owner;
    protected AbilityDeck deck;
    protected List<Card> hand;
    #endregion
    #region MonoBehaviour
    void Start()
    {
        owner = GetComponentInParent<Unit>();
        if (owner.hand == null)
            owner.hand = new List<Card>();
        deck = owner.deck;
        hand = owner.hand;
        while (hand.Count < 4)
        {
            hand.Add(deck.Draw());
        }
    }
    #endregion
    #region Public
    public abstract void Pick(PlanOfAttack plan);
    #endregion

    #region Protected
    protected Card Find(string cardName)
    {
        for (int i = 0; i < hand.Count; ++i)
        {
            if (hand[i] != null && hand[i].GetName() == cardName)
                return hand[i];
        }
        return null;
    }
    protected Card Default()
    {
        return hand[0];
    }
    #endregion
}
