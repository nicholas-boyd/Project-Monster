using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class AbilityCardPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image cardBack;
    [SerializeField] Image cardImage;
    [SerializeField] Image rangeIndicator;
    [SerializeField] Image manaIndicator;
    [SerializeField] Image powerIndicator;
    Sprite normalSprite;
    Sprite disabledSprite;
    Sprite selectedSprite;
    [SerializeField] Text Name;
    [SerializeField] Text ManaCost;
    [SerializeField] Text Power;
    [SerializeField] Text NumTargets;
    public Panel panel;
    public Card card;
    Outline cardOutline;
    Outline nameOutline;
    public bool pointerOver = false;

    void Awake()
    {
        nameOutline = Name.GetComponent<Outline>();
        cardOutline = cardBack.GetComponent<Outline>();
        cardBack.color = Color.clear;
        cardImage.color = Color.clear;
        rangeIndicator.color = Color.clear;
        manaIndicator.color = Color.clear;
        powerIndicator.color = Color.clear;
    }

    public string CardTitle
    {
        get { return Name.text; }
        set { Name.text = value; }
    }

    public Image CardImage
    {
        get { return cardImage; }
        set { cardImage = value; }
    }

    public void Display(CardRecipe data)
    {
        cardBack.color = Color.white;
        if (data.AbilityPower.StartsWith("Physical"))
            cardBack.color = Color.red;
        if (data.AbilityPower.StartsWith("Special"))
            cardBack.color = Color.blue;

        cardImage.color = Color.white;
        rangeIndicator.color = Color.white;
        if (cardBack.sprite == null)
        {
            cardBack.sprite = Resources.Load<Sprite>("Sprites/Cards/cardBack");
        }

        if(manaIndicator.color.Equals(Color.clear) || powerIndicator.color.Equals(Color.clear))
        {
            manaIndicator.sprite = Resources.Load<Sprite>("Sprites/Cards/Mana");
            manaIndicator.color = Color.white;
            powerIndicator.sprite = Resources.Load<Sprite>("Sprites/Cards/Damage");
            powerIndicator.color = Color.white;
        }

        card.Load(data);
        normalSprite = data.Sprite;
        //TODO
        disabledSprite = normalSprite;
        selectedSprite = normalSprite;

        cardImage.sprite = normalSprite;
        cardImage.SetNativeSize();

        rangeIndicator.sprite = data.AbilityRangeIndicator;
        rangeIndicator.SetNativeSize();

        Name.text = card.GetName();
        ManaCost.text = card.GetComponent<AbilityManaCost>().amount.ToString();
        Power.text = card.GetComponent<BaseAbilityPower>().Power.ToString();
        int numTargets = card.GetComponent<AbilityEffectTarget>().maxTargets();
        if (numTargets == int.MaxValue)
            NumTargets.text = "∞";
        else
            NumTargets.text = numTargets.ToString();
        if (data.Name.Equals(""))
        {
            rangeIndicator.sprite = cardImage.sprite;
            ManaCost.text = null;
            Power.text = null;
            NumTargets.text = null;
            manaIndicator.sprite = null;
            manaIndicator.color = Color.clear;
            powerIndicator.sprite = null;
            powerIndicator.color = Color.clear;
        }

    }

    [System.Flags]
    enum States
    {
        None = 0,
        Selected = 1 << 0,
        Locked = 1 << 1
    }

    public bool IsLocked
    {
        get { return (State & States.Locked) != States.None; }
        set
        {
            if (value)
                State |= States.Locked;
            else
                State &= ~States.Locked;
        }
    }

    public bool IsSelected
    {
        get { return (State & States.Selected) != States.None; }
        set
        {
            if (value)
                State |= States.Selected;
            else
                State &= ~States.Selected;
        }
    }

    States State
    {
        get { return state; }
        set
        {
            if (state == value)
                return;
            state = value;

            if (IsLocked)
            {
                cardImage.sprite = disabledSprite;
                Name.color = Color.gray;
                nameOutline.effectColor = new Color32(20, 36, 44, 255);
                cardOutline.effectColor = new Color32(20, 36, 44, 255);
                cardOutline.effectDistance = new Vector2(1, -1);
            }
            else if (IsSelected)
            {
                cardImage.sprite = selectedSprite;
                nameOutline.effectColor = new Color32(255, 160, 72, 255);
                cardOutline.effectColor = new Color32(255, 160, 72, 255);
                cardOutline.effectDistance = new Vector2(3, -3);
            }
            else
            {
                cardImage.sprite = normalSprite;
                nameOutline.effectColor = new Color32(20, 36, 44, 255);
                cardOutline.effectColor = new Color32(224, 207, 180, 128);
                cardOutline.effectDistance = new Vector2(0, 0);
            }
        }
    }
    States state;

    public void Reset()
    {
        State = States.None;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
    }
}