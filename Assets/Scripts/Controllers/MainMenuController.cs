﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public TransitionPanel transition;
    public MainMenuPanel menuPanel;
    public bool hidden;

    public const string TransitionNotification = "TRANSITION_PANEL_ACTIVATE";

    string ShowKey = "Show";
    string HideKey = "Hide";

    void Start()
    {
        hidden = false;
        menuPanel.panel.SetPosition(ShowKey, false);
    }
    void OnEnable()
    {
        this.AddObserver(ResizeScreen, InputController.ScreenSizeUpdateNotification);
    }
    void OnDisable()
    {
        this.RemoveObserver(ResizeScreen, InputController.ScreenSizeUpdateNotification);
    }

    void ResizeScreen(object sender, object args)
    {
        if (hidden)
        {
            menuPanel.panel.SetPosition(HideKey, false);
        }
        else
        {
            menuPanel.panel.SetPosition(ShowKey, false);
        }
    }

    public void OnPlayButton()
    {
        hidden = true;
        menuPanel.panel.SetPosition(HideKey, true);
        this.PostNotification(TransitionNotification, true);
    }
    public void OnExitButton()
    {
        Application.Quit();
    }
}
