using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleClockController : MonoBehaviour
{
    public const string NewSecondNotification = "NEW_SECOND_NOTIFICATION";
    public const string NewMinuteNotification = "NEW_MINUTE_NOTIFICATION";

    [SerializeField] Text text;
    [SerializeField] GameObject canvas;
    [SerializeField] CanvasGroup group;

    EasingControl easing;

    public int MinutesPassed
    {
        get { return Mathf.FloorToInt(SecondsPassed/60); }
    }

    public float SecondsPassed
    {
        get { return _seconds; }
    }

    protected float _seconds;
    protected float secondsTracker;
    protected float minutesTracker;

    public void Activate()
    {
        canvas.SetActive(true);
        if (!easing)
            easing = gameObject.AddComponent<EasingControl>();
        easing.duration = 1f;
        easing.equation = EasingEquations.EaseInOutQuad;
        easing.endBehaviour = EasingControl.EndBehaviour.Constant;
        easing.updateEvent += OnUpdateEvent;
        easing.Play();

        _seconds = 0;
        secondsTracker = _seconds;
        minutesTracker = _seconds;

        text.text = string.Format("{0}:{1}", MinutesPassed, System.Math.Round(Mathf.Clamp(SecondsPassed - (MinutesPassed * 60), 0, 59.99f), 2));
    }

    private void Start()
    {
        group.alpha = 0;

        _seconds = 0;
        secondsTracker = _seconds;
        minutesTracker = _seconds;
        
        text.text = string.Format("{0}:{1}", MinutesPassed, System.Math.Round(Mathf.Clamp(SecondsPassed - (MinutesPassed * 60), 0, 59.99f), 2));
        canvas.SetActive(false);
    }

    private void Update()
    {
        _seconds += Time.deltaTime;

        if (_seconds - secondsTracker >= 1)
        {
            this.PostNotification(NewSecondNotification, SecondsPassed);
            Debug.Log("Second Passed");
            secondsTracker = _seconds;
        }
        
        if (MinutesPassed - minutesTracker >= 1)
        {
            this.PostNotification(NewMinuteNotification, MinutesPassed);
            Debug.Log("Minute Passed");
            minutesTracker = MinutesPassed;
        }

        text.text = string.Format("{0}:{1:0.00}", MinutesPassed, Mathf.Clamp(SecondsPassed - (MinutesPassed * 60), 0, 59.99f));
    }

    void OnUpdateEvent(object sender, EventArgs e)
    {
        group.alpha = easing.currentValue;
    }
}
