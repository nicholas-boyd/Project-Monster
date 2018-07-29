using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDeck
{

    public Stack<Card> deck;

    public AbilityDeck(List<Card> cards)
    {
        SetDeck(cards);
    }

    public int Count
    {
        get { return this.deck.Count; }
    }

    public void Clear()
    {
        for (int i = 0; i < deck.Count; ++i)
        {
            deck.Pop();
        }
    }

    public void Add(Card card)
    {
        if (card != null)
            deck.Push(card);
    }

    public void Add(AbilityDeck ad)
    {
        while (ad.Count != 0)
        {
            Add(ad.Draw());
        }
    }

    public void PlaceAt(Card card, int index)
    {
        if (index < 0 || index > deck.Count)
            return;
        List<Card> oldDeck = new List<Card>(deck.ToArray());
        oldDeck.Insert(index, card);
        SetDeck(oldDeck);

    }

    public void Remove(Card card)
    {
        if (!deck.Contains(card))
            return;
        List<Card> oldDeck = new List<Card>(deck.ToArray());
        oldDeck.Remove(card);
        SetDeck(oldDeck);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index > deck.Count)
            return;
        List<Card> oldDeck = new List<Card>(deck.ToArray());
        oldDeck.RemoveAt(index);
        SetDeck(oldDeck);
    }

    public void Shuffle()
    {
        Stack<Card> newDeck = new Stack<Card>();
        List<Card> oldDeck = new List<Card>(deck.ToArray());
        for (int i = 0; i < deck.Count; ++i)
        {
            int r = Random.Range(0, oldDeck.Count - 1);
            newDeck.Push(oldDeck[r]);
            oldDeck.RemoveAt(r);
        }
        SetDeck(newDeck);
    }

    public Card Draw()
    {
        if (deck.Count > 0)
            return deck.Pop();
        else
            return null;
    }

    void SetDeck(List<Card> list)
    {
        if (deck == null)
            deck = new Stack<Card>();
        deck.Clear();
        for (int i = 0; i < list.Count; ++i)
        {
            deck.Push(list[i]);
        }
    }

    void SetDeck(Stack<Card> list)
    {
        if (deck == null)
            deck = new Stack<Card>();
        deck = list;
    }
}
