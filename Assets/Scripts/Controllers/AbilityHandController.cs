using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
public class AbilityHandController : MonoBehaviour
{
    public const string AbilityCardSelectedNotification = "ABILITYCARD_SELECTED";

    // Show, and Hide Keys for handPanel. Card, Deck, and Grave Keys for cardPanels.
    const string ShowKey = "Show";
    const string HideKey = "Hide";
    const string DeckKey = "Deck";
    const string Card0Key = "Card 0";
    const string Card1Key = "Card 1";
    const string Card2Key = "Card 2";
    const string Card3Key = "Card 3";
    const string Card4Key = "Card 4";
    const string Card5Key = "Card 5";
    const string DiscardKey = "Grave";

    [SerializeField] BattleController owner;
    [SerializeField] GameObject handCanvas;
    [SerializeField] List<AbilityCardPanel> cardPanels;
    [SerializeField] AbilityDeckPanel DiscardPanel;
    [SerializeField] AbilityDeckPanel DeckPanel;
    [SerializeField] ReloadTimerPanel ReloadTimer;
    [SerializeField] Panel handPanel;
    Tweener[] transitions;

    AbilityDeck deck;
    AbilityDeck discards;
    List<Card> hand;
    public const int MaxHandSize = 6;
    public int handSize = 0;
    public int curHandSize = 0;

    public int selection { get; private set; }
    float drawCooldown;
    bool hidden = true;
    bool redrawing = false;
    bool reshuffling = false;
    int MaxScrollDuration = 10;
    int ScrollDuration = 10;

    void OnEnable()
    {
        this.AddObserver(InputAlphaHeld, InputController.AlphaHeldNotification);
        this.AddObserver(InputAlphaUp, InputController.AlphaUpNotification);
        this.AddObserver(InputNumberUp, InputController.NumberUpNotification);
        this.AddObserver(InputScroll, InputController.Scrolling);


        this.AddObserver(CheckSelectionTarget, CombatBattleState.CheckSelectionTargetNotification);
        this.AddObserver(CheckLayout, InputController.ScreenSizeUpdateNotification);
    }

    void OnDisable()
    {
        this.RemoveObserver(InputAlphaHeld, InputController.AlphaHeldNotification);
        this.RemoveObserver(InputAlphaUp, InputController.AlphaUpNotification);
        this.RemoveObserver(InputNumberUp, InputController.NumberUpNotification);
        this.RemoveObserver(InputScroll, InputController.Scrolling);


        this.RemoveObserver(CheckSelectionTarget, CombatBattleState.CheckSelectionTargetNotification);
        this.RemoveObserver(CheckLayout, InputController.ScreenSizeUpdateNotification);
    }

    // Decreases reload timer if R is held
    void InputAlphaHeld(object sender, object args)
    {
        if (args.Equals(KeyCode.R))
        {
            if (drawCooldown > Time.deltaTime)
                drawCooldown -= Time.deltaTime;
            else
                drawCooldown = 0;
            ReloadTimer.Display(drawCooldown);
        }
    }

    // Resets draw cooldown if R is no longer held
    void InputAlphaUp(object sender, object args)
    {
        if (args.Equals(KeyCode.R))
            SetDrawCooldown();
    }

    // Sets selection if number input is given
    void InputNumberUp(object sender, object args)
    {
        SetSelection((int)args - 1);
    }

    // Scrolls through selections if scroll input is given
    void InputScroll(object sender, object args)
    {
        if (ScrollDuration <= 0)
        {
            if ((int)args < 0)
            {
                Next();
            }
            else
            {
                Previous();
            }
            ScrollDuration = MaxScrollDuration;
        }
    }

    // Checks if the mouse is over any card panels
    void CheckSelectionTarget(object sender, object args)
    {
        List<int> activePosition = args as List<int>;
        for (int i = 0; i < handSize; i++)
            if (cardPanels[i].pointerOver)
                activePosition.Add(i);
    }

    void CheckLayout(object sender, object args)
    {
        handPanel.SetPosition(HideKey, true);
        if (!hidden)
        {
            MatchScreen();
            ActivatePanel();
        }
    }


    // Animation: moves card panel at index i to discards, adds the panel's card to discards, and fills the panel with an empty card before moving it back
    IEnumerator DiscardEntry(int i)
    {
        if (i >= 0 && i < handSize && hand[i] != null && hand[i].GetName() != "")
        {
            yield return StartCoroutine(MoveAbilityPanel(cardPanels[i], DiscardKey));
            discards.Add(hand[i]);
            hand[i] = EmptyCard();
            curHandSize--;
            RefreshEntry(i);
            DiscardPanel.Display(discards.Count);
            yield return StartCoroutine(MoveAbilityPanel(cardPanels[i], GetCardKey(i)));
        }
        SetDrawCooldown();
    }

    // Draws a card at nearest empty location to i
    IEnumerator DrawCard(int i)
    {
        if (i >= 0 && i < handSize && hand != null)
        {
            if (hand[i] != null)
            {
                if (hand[i].GetName() != "")
                {
                    yield return RefreshAbilityPanel(i);
                    yield break;
                }
            }
            yield return StartCoroutine(MoveAbilityPanel(cardPanels[i], DeckKey));
            hand[i] = deck.Draw();
            curHandSize++;
            RefreshEntry(i);
            DeckPanel.Display(deck.Count);
            if (deck.Count < handSize)
                DeckPanel.Display(0);
            yield return StartCoroutine(MoveAbilityPanel(cardPanels[i], GetCardKey(i)));
            SetDrawCooldown();
        }

    }

    // Helper animation method for adjusting tweeners, and assigning destinations
    IEnumerator MoveAbilityPanel(AbilityCardPanel obj, string pos)
    {
        obj.transform.SetAsLastSibling();
        int index = cardPanels.IndexOf(obj);
        transitions[index] = obj.panel.SetPosition(pos, true);
        transitions[index].easingControl.duration = 0.25f;
        transitions[index].easingControl.equation = EasingEquations.EaseOutQuint;
        while (transitions[index] != null)
            yield return null;
    }

    // Helper animation method for adjusting tweeners, and assigning destinations
    IEnumerator MoveDeckPanel(AbilityDeckPanel obj, string pos)
    {
        obj.transform.SetAsLastSibling();
        int index = obj == DeckPanel ? 6 : 7;
        transitions[index] = obj.panel.SetPosition(pos, true);
        if (pos.Equals(DeckKey))
        {
            transitions[index].easingControl.equation = EasingEquations.EaseOutBounce;
            transitions[index].easingControl.duration = .5f;
        }
        else
        {
            transitions[index].easingControl.equation = EasingEquations.EaseOutCubic;
            transitions[index].easingControl.duration = .75f;
        }
        while (transitions[index] != null)
            yield return null;
    }

    // Discard then draw combo method
    IEnumerator RefreshAbilityPanel(int i)
    {
        while (reshuffling)
        {
            yield return new WaitForSeconds(.2f);
        }
        if (cardPanels[i].card != null && cardPanels[i].card.GetName() != "")
            yield return StartCoroutine(DiscardEntry(i));
        yield return StartCoroutine(DrawCard(i));
        if (i == handSize - 1)
            redrawing = false;
    }

    // Shuffle Discards back into deck
    IEnumerator ReshuffleDiscards()
    {
        reshuffling = true;
        yield return StartCoroutine(MoveDeckPanel(DiscardPanel, DeckKey));
        deck.Add(discards);
        deck.Shuffle();
        DeckPanel.Display(deck.Count);
        discards = new AbilityDeck(new List<Card>());
        DiscardPanel.Display(discards.Count);
        yield return StartCoroutine(MoveDeckPanel(DiscardPanel, DiscardKey));
        reshuffling = false;
    }

    // On object creation
    void Start()
    {
        // Set up empty lists
        transitions = new Tweener[8];
        handPanel.SetPosition(HideKey, false);
        hidden = true;
    }

    // Connects player variables, draws first hand, shows hand panel
    public void ActivatePanel()
    {
        // Show hand panel to set up card panel positions
        handPanel.SetPosition(ShowKey, false);
        hidden = false;
        MatchScreen();

        // Lock panels beyond handsize, and properly display deck and graveyard panels
        for (int i = handSize; i < cardPanels.Count; i++)
            cardPanels[i].IsLocked = true;
        DiscardPanel.Display(0);
        DeckPanel.Display(0);

        // Hide hand until activation
        handPanel.SetPosition(HideKey, false);

        SetDrawCooldown();

        // Set up (connect) deck, and hand variables with player
        owner.playerUnit.hand = new List<Card>(MaxHandSize);
        hand = owner.playerUnit.hand;
        discards = owner.playerUnit.discards;
        deck = owner.playerUnit.deck;
        handSize = owner.playerUnit.handSize;
        curHandSize = handSize;

        // Draw hand
        for (int i = 0; i < MaxHandSize; ++i)
        {
            if (i >= curHandSize)
            {
                if (!cardPanels[i].IsLocked)
                    cardPanels[i].IsLocked = true;
                cardPanels[i].card = EmptyCard();
            }
            else
            {
                deck.Shuffle();
                hand.Add(deck.Draw());
            }

            RefreshEntry(i);
        }

        DeckPanel.Display(deck.Count);
        SetSelection(0);
        SetDrawCooldown();

        handPanel.SetPosition(ShowKey, true);
        hidden = false;
    }



    // Get Card Key string by index
    string GetCardKey(int i)
    {
        switch (i)
        {
            case 0:
                return Card0Key;
            case 1:
                return Card1Key;
            case 2:
                return Card2Key;
            case 3:
                return Card3Key;
            case 4:
                return Card4Key;
            case 5:
                return Card5Key;
        }
        return null;
    }

    Card EmptyCard()
    {
        Card c = Instantiate(Resources.Load("Prefabs/AbilityCards/Empty Card") as GameObject).GetComponent<Card>();
        c.transform.parent = owner.playerUnit.transform;
        c.Load(null);
        return c;
    }

    void MatchScreen()
    {
        int cardWidth = Mathf.RoundToInt(DeckPanel.panel.GetComponentInChildren<RectTransform>().rect.width);
        int offset = Mathf.RoundToInt((handCanvas.GetComponent<RectTransform>().rect.width - cardWidth * 8) / 9);
        offset = offset < 0 ? 0 : offset;
        int curOffset = offset;
        int graveOffset = offset + ((offset + cardWidth) * 7);

        // Deck Panel only needs its own position, which is reset here
        DeckPanel.panel.RemovePosition(DeckKey);
        DeckPanel.panel.AddPosition(DeckKey, TextAnchor.LowerLeft, TextAnchor.LowerLeft, new Vector2(offset, 0));
        DeckPanel.panel.SetPosition(DeckKey, true);

        // Graveyard Panel will only need its own position and Deck's position which is set here
        DiscardPanel.panel.RemovePosition(DiscardKey);
        DiscardPanel.panel.AddPosition(DiscardKey, TextAnchor.LowerLeft, TextAnchor.LowerLeft, new Vector2(graveOffset, 0));
        DiscardPanel.panel.AddPosition(DeckKey, TextAnchor.LowerLeft, TextAnchor.LowerLeft, new Vector2(offset, 0));
        DiscardPanel.panel.SetPosition(DiscardKey, true);

        curOffset += cardWidth + offset;

        for (int i = 0; i < MaxHandSize; i++)
        {
            // Reset Deck, Grave, and current Panel's position on current Panel
            cardPanels[i].panel.RemovePosition(DeckKey);
            cardPanels[i].panel.RemovePosition(DiscardKey);
            cardPanels[i].panel.RemovePosition(GetCardKey(i));
            cardPanels[i].panel.AddPosition(DeckKey, TextAnchor.LowerLeft, TextAnchor.LowerLeft, new Vector2(offset, 0));
            cardPanels[i].panel.AddPosition(DiscardKey, TextAnchor.LowerLeft, TextAnchor.LowerLeft, new Vector2(graveOffset, 0));
            cardPanels[i].panel.AddPosition(GetCardKey(i), TextAnchor.LowerLeft, TextAnchor.LowerLeft, new Vector2(curOffset, 0));

            // Reset current Panel's position on all other Card Panels
            for (int j = 0; j < MaxHandSize; j++)
            {
                if (j != i)
                {
                    cardPanels[j].panel.RemovePosition(GetCardKey(i));
                    cardPanels[j].panel.AddPosition(GetCardKey(i), TextAnchor.LowerLeft, TextAnchor.LowerLeft, new Vector2(curOffset, 0));
                }
            }

            // Ensure current Panel's position is correct, set offset for next Panel
            cardPanels[i].panel.SetPosition(GetCardKey(i), true);
            curOffset += cardWidth + offset;
        }
    }

    // Update card display if it doesn't match its assigned card in hand
    void RefreshEntry(int i)
    {
        if (i > -1 && i < MaxHandSize)
        {
            if (i < handSize)
            {
                cardPanels[i].card = hand[i];
            }
            cardPanels[i].Display(cardPanels[i].card.GetData());
        }
    }

    // Refreshes the value of the reload timer
    void SetDrawCooldown()
    {
        drawCooldown = 1;
        for (int i = 0; i < handSize; i++)
            if (cardPanels[i].card != null && cardPanels[i].card.GetName() != "")
                drawCooldown += 0.2f;
        if (deck != null && deck.Count < handSize)
            drawCooldown += 1.0f;
        ReloadTimer.Display(drawCooldown);// - (owner.playerUnit.GetComponent<Dexterity>().DEX * 0.25f);
    }

    //Called every frame
    void Update()
    {
        // Redraw hand if cooldown done
        bool transitionsDone = transitions[0] == null && transitions[1] == null && transitions[2] == null && transitions[3] == null && transitions[4] == null && transitions[5] == null && transitions[6] == null && transitions[7] == null;
        if (Mathf.Approximately(drawCooldown, 0) && !redrawing && transitionsDone && handPanel.CurrentPosition.name.Equals(ShowKey))
        {
            redrawing = true;
            if (deck != null && deck.Count < handSize)
            {
                StartCoroutine(ReshuffleDiscards());
            }
            for (int i = 0; i < cardPanels.Count; i++)
            {
                if (i < handSize)
                {
                    StartCoroutine(RefreshAbilityPanel(i));
                }
            }
        }
        ScrollDuration--;
    }

    // Activates currently selected card ability
    public void ActivateSelection()
    {
        Card c = hand[selection];
        if (c.GetComponent<AbilityManaCost>().amount < owner.playerUnit.GetComponent<Mana>().MP && !c.GetName().Equals(""))
        {
            List<Tile> targets = c.GetComponentInChildren<AbilityRange>().GetTilesInRange(owner.board);
            this.PostNotification(AbilityCardSelectedNotification, c);
            hand[selection].Perform(targets);
            StartCoroutine(DiscardEntry(selection));
            DiscardPanel.Display(discards.Count);
            Next();
        }
    }

    // Select next available Card Panel
    public void Next()
    {
        for (int i = selection + 1; i < selection + hand.Count; ++i)
        {
            int index = i % hand.Count;
            if (SetSelection(index))
                break;
        }
    }

    // Select previous available Card Panel
    public void Previous()
    {
        for (int i = selection - 1 + hand.Count; i > selection; --i)
        {
            int index = i % hand.Count;
            if (SetSelection(index))
                break;
        }
    }

    // Refreshes Range Indicators for currently selected card
    public void RefreshMarkers()
    {
        GameObject[] dels = GameObject.FindGameObjectsWithTag("Range Marker");
        foreach (GameObject d in dels)
            Destroy(d, 0);
        List<Tile> newTargets = hand[selection].GetComponent<AbilityRange>().GetTilesInRange(owner.board);
        foreach (Tile nt in newTargets)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/ParticleEffects/FinalizedEffects/particle_hitbox");
            GameObject instance = GameObject.Instantiate(prefab);
            instance.transform.SetParent(owner.transform.Find("Range Markers"));
            instance.name = "Range Marker";
            instance.transform.localPosition = nt.center;
        }
    }

    // Locks or unlocks panel at index
    public void SetLocked(int index, bool value)
    {
        if (index < 0 || index >= hand.Count)
            return;
        cardPanels[index].IsLocked = value;
        RefreshEntry(index);
        if (value && selection == index)
            Next();
    }

    // Sets Empty Card, and locks panel at index
    /*public void SetEmpty(int index)
    {
        if (index < 0 || index >= hand.Count)
            return;
        Card card = hand[index];
        if (card == null)
            card = Instantiate(Resources.Load("Prefabs/AbilityCards/Empty Card") as GameObject).GetComponent<Card>();
        CardRecipe data = Resources.Load("Recipes/CardRecipes/Empty Card Data") as CardRecipe;
        card.name = "LOCKED";
        card.transform.SetParent(handCanvas.transform);
        card.Load(data);
        cardPanels[index].card = card;
        RefreshEntry(index);
        if (selection == index)
            Next();
    }*/

    // Set Selection: the index of the currently selected card
    public bool SetSelection(int value)
    {
        if (cardPanels[value].IsLocked || cardPanels[value].card.GetName() == "")
            return false;

        // Deselect the previously selected entry
        if (selection >= 0 && selection < hand.Count)
            cardPanels[selection].IsSelected = false;

        RefreshEntry(selection);
        selection = value;

        // Select the new entry
        if (selection >= 0 && selection < hand.Count)
            cardPanels[selection].IsSelected = true;

        RefreshEntry(selection);
        RefreshMarkers();
        return true;
    }


}