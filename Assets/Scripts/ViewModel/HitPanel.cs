using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitPanel : MonoBehaviour {

    [SerializeField] Text text;
    [SerializeField] GameObject canvas;
    [SerializeField] CanvasGroup group;
    EasingControl easing;
    bool SequencePlaying;
    Queue<string> toDisplay;


    void Start()
    {
        if (!text)
            text = GetComponentInChildren<Text>();
        if (!canvas)
            canvas = GetComponentInChildren<Canvas>().gameObject;
        if (!group)
            group = GetComponentInChildren<CanvasGroup>();
        text.text = "";
        canvas.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    void Awake()
    {
        group.alpha = 0;
        toDisplay = new Queue<string>();
        easing = gameObject.AddComponent<EasingControl>();
        easing.duration = 0.2f;
        easing.equation = EasingEquations.EaseInOutQuad;
        easing.endBehaviour = EasingControl.EndBehaviour.Constant;
        easing.updateEvent += OnUpdateEvent;
        SequencePlaying = false;
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
    void AddDamageEffectToQueue(object sender, object target)
    {
        Info<Unit, Unit, int> info = target as Info<Unit, Unit, int>;
        if (info.arg1.Equals(GetComponentInParent<Unit>()))
            toDisplay.Enqueue(string.Format("-{0}", Mathf.Abs(info.arg2)));
    }
    void AddHealEffectToQueue(object sender, object target)
    {
        Info<Unit, Unit, int> info = target as Info<Unit, Unit, int>;
        if (info.arg1.Equals(GetComponentInParent<Unit>()))
            toDisplay.Enqueue(string.Format("+{0}", Mathf.Abs(info.arg2)));
    }
    void Update()
    {
        if (transform.parent.rotation.y != 0)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        else
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        if (!SequencePlaying && toDisplay.Count != 0)
        {
            Display(toDisplay.Dequeue());
        }
    }
    public void Display(string message)
    {
        group.alpha = 0;
        canvas.SetActive(true);
        if (message.StartsWith("-"))
            text.color = Color.red;
        else
            text.color = Color.green;
        text.text = message;
        StartCoroutine(Sequence());
    }
    void OnUpdateEvent(object sender, EventArgs e)
    {
        group.alpha = easing.currentValue;
    }
    IEnumerator Sequence()
    {
        SequencePlaying = true;
        easing.Play();
        while (easing.IsPlaying)
            yield return null;
        yield return new WaitForSeconds(0.2f);
        easing.Reverse();
        while (easing.IsPlaying)
            yield return null;
        canvas.SetActive(false);
        SequencePlaying = false;
    }

}
