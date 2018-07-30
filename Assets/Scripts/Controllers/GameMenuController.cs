using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuController : MonoBehaviour {
    
    public GameMenuPanel menuPanel;
    public InventoryPanelController inventoryPanelController;
    public bool hidden;

    public const string TransitionNotification = "TRANSITION_PANEL_ACTIVATE";

    string ShowKey = "Show";
    string HideKey = "Hide";

    void Start()
    {
        RectTransform rt = menuPanel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        hidden = true;
        menuPanel.panel.SetPosition(HideKey, false);
    }

    void OnEnable()
    {
        this.AddObserver(InventoryToggle, InputController.AlphaDownNotification);
        this.AddObserver(CheckLayout, InputController.ScreenSizeUpdateNotification);
    }

    void OnDisable()
    {
        this.RemoveObserver(InventoryToggle, InputController.AlphaDownNotification);
        this.RemoveObserver(CheckLayout, InputController.ScreenSizeUpdateNotification);
    }

    void InventoryToggle(object sender, object args)
    {
        if (args.Equals(KeyCode.I))
            OnInventoryButton();
    }

    void CheckLayout(object sender, object args)
    {
        if (hidden)
            menuPanel.panel.SetPosition(HideKey, false);
        else
            menuPanel.panel.SetPosition(ShowKey, false);
    }

    public void Show()
    {
        hidden = false;
        menuPanel.panel.SetPosition(ShowKey, true);
    }

    public void Hide()
    {
        this.PostNotification(TransitionNotification, false);
        hidden = true;
        menuPanel.panel.SetPosition(HideKey, true);
    }

    public void OnInventoryButton()
    {
        inventoryPanelController.Toggle();
    }

    public void OnDeckButton()
    {

    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
