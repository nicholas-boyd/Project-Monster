using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class BattleMessageController : MonoBehaviour
{
    [SerializeField] Text label;
    [SerializeField] GameObject canvas;
    [SerializeField] CanvasGroup group;
    EasingControl ec;
    bool SequencePlaying;
    Queue<string> toDisplay;
    float displayTime;
    void Awake()
    {
        group.alpha = 0;
        toDisplay = new Queue<string>();
        ec = gameObject.AddComponent<EasingControl>();
        ec.duration = 0.2f;
        ec.equation = EasingEquations.EaseInOutQuad;
        ec.endBehaviour = EasingControl.EndBehaviour.Constant;
        ec.updateEvent += OnUpdateEvent;
        SequencePlaying = false;
        displayTime = 0.2f;
    }
    void OnEnable()
    {
        this.AddObserver(AddDamageEffectToQueue, DamageAbilityEffect.DamageDealtNotification);
        this.AddObserver(AddHealEffectToQueue, HealAbilityEffect.UnitHealedNotification);
    }

    void OnDisable()
    {
        this.RemoveObserver(AddDamageEffectToQueue, DamageAbilityEffect.DamageDealtNotification);
        this.RemoveObserver(AddHealEffectToQueue, HealAbilityEffect.UnitHealedNotification);
    }
    void AddAbilityActivationToQueue(object sender, object target)
    {
        Info<Unit, Card> info = target as Info<Unit, Card>;
        toDisplay.Enqueue(string.Format("{0} used {1}", info.arg0.name, info.arg1.GetName()));
    }
    void AddDamageEffectToQueue(object sender, object target)
    {
        Info<Unit, Unit, int> info = target as Info<Unit, Unit, int>;
        toDisplay.Enqueue(string.Format("{0} hit {1} for {2} damage", info.arg0.name, info.arg1.name, Mathf.Abs(info.arg2)));
    }
    void AddHealEffectToQueue(object sender, object target)
    {
        Info<Unit, Unit, int> info = target as Info<Unit, Unit, int>;
        if (info.arg0 == info.arg1)
            toDisplay.Enqueue(string.Format("{0} healed themself for {1} HP", info.arg0.name, Mathf.Abs(info.arg2)));
        else
            toDisplay.Enqueue(string.Format("{0} healed {1} for {2} HP", info.arg0.name, info.arg1.name, Mathf.Abs(info.arg2)));
    }
    void Start()
    {
        label.text = "";
    }
    void Update()
    {
        if (!SequencePlaying && toDisplay.Count != 0)
        {
            Display(toDisplay.Dequeue());
        }
    }
    public void Display(string message)
    {
        group.alpha = 0;
        canvas.SetActive(true);
        label.text = message;
        StartCoroutine(Sequence());
    }
    void OnUpdateEvent(object sender, EventArgs e)
    {
        group.alpha = ec.currentValue;
    }
    IEnumerator Sequence()
    {
        SequencePlaying = true;
        ec.Play();
        while (ec.IsPlaying)
            yield return null;
        if (toDisplay.Count > 1 && displayTime > 0)
            displayTime -= 0.075f;
        else if (toDisplay.Count == 0)
            displayTime = 0.2f;
        yield return new WaitForSeconds(displayTime);
        ec.Reverse();
        while (ec.IsPlaying)
            yield return null;
        canvas.SetActive(false);
        SequencePlaying = false;
    }
}