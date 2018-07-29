using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StatPanel : MonoBehaviour
{
    public Panel panel;
    public Sprite allyBackground;
    public Sprite enemyBackground;
    public Image background;
    public Image icon;
    public Image statusIndicator;
    public Text nameLabel;
    public Text hpLabel;
    public Text mpLabel;


    Status displayStatus;
    List<StatusEffect> displayEffects;

    bool showingEffects = false;

    void Start()
    {
        displayEffects = new List<StatusEffect>();
        statusIndicator.color = Color.clear;
    }

    void Update()
    {
        if (displayStatus && displayEffects.Count > 0 && !showingEffects)
        {
            displayEffects = displayStatus.effects;
            showingEffects = true;
            StartCoroutine(ShowEffects());
        }
    }

    IEnumerator ShowEffects()
    {
        statusIndicator.color = Color.white;
        while (displayEffects.Count > 0)
        {
            for (int i = 0; i < displayEffects.Count; i++)
            {
                statusIndicator.sprite = displayEffects[i].statusEffectIcon;
                displayEffects = displayStatus.effects;
                if (displayEffects.Count != 0)
                    yield return new WaitForSeconds(0.5f);
            }
        }
        statusIndicator.sprite = null;
        statusIndicator.color = Color.clear;
        showingEffects = false;
    }

    public void Display(GameObject obj)
    {
        background.sprite = obj.GetComponent<Alliance>().type == Alliances.Hero ? allyBackground : enemyBackground;
        icon.preserveAspect = true;
        icon.sprite = obj.GetComponent<SpriteRenderer>().sprite;
        //icon.SetNativeSize();
        nameLabel.text = obj.name;
        Stats stats = obj.GetComponent<Stats>();
        if (stats)
        {
            hpLabel.text = string.Format("HP {0} / {1}", Mathf.RoundToInt(stats[StatTypes.HP]), stats[StatTypes.MHP]);
            mpLabel.text = string.Format("MP {0} / {1}", Mathf.RoundToInt(stats[StatTypes.MP]), stats[StatTypes.MMP]);
        }

        Status status = obj.GetComponent<Status>();
        if (status.effects.Count == 0)
        {
            displayEffects = new List<StatusEffect>();
            statusIndicator.sprite = null;
            statusIndicator.color = Color.clear;
        }
        else
        {
            displayStatus = status;
            displayEffects = status.effects;
        }
    }
}