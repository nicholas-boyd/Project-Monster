using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InventoryPanelController : MonoBehaviour
{
    #region Const
    const string ShowKey = "Show";
    const string HideKey = "Hide";
    #endregion
    #region Fields
    public Vector3[] itemPositions;
    [SerializeField] InventoryPanel panel;
    public bool Show = false;

    Tweener tweener;
    #endregion
    #region MonoBehaviour
    void Start()
    {
        panel.panel.SetPosition(ShowKey, false);
        
        itemPositions = new Vector3[panel.items.Count];


        for (int i = 0; i < panel.items.Count; i++)
        {
            itemPositions[i] = panel.items[i].transform.position;
        }

        panel.panel.SetPosition(HideKey, false);
    }

    void OnEnable()
    {
        this.AddObserver(ItemPanelUpdate, ItemPanel.ItemPanelDragNotification);
        this.AddObserver(CheckLayout, InputController.ScreenSizeUpdateNotification);
    }

    void OnDisable()
    {
        this.RemoveObserver(ItemPanelUpdate, ItemPanel.ItemPanelDragNotification);
        this.RemoveObserver(CheckLayout, InputController.ScreenSizeUpdateNotification);
    }
    #endregion
    #region Public
    public void ShowInPanel(Equippable obj, int index)
    {
        panel.Display(obj, index);
    }
    public void ShowPanel()
    {
        tweener = panel.panel.SetPosition(ShowKey, true);
        tweener.easingControl.duration = 0.5f;
        tweener.easingControl.equation = EasingEquations.EaseOutQuad;
        MovePanel(panel, ShowKey, tweener);
        Show = true;
    }
    public void HidePanel()
    {
        tweener = panel.panel.SetPosition(HideKey, true);
        tweener.easingControl.duration = 0.5f;
        tweener.easingControl.equation = EasingEquations.EaseOutQuad;
        MovePanel(panel, HideKey, tweener);
        Show = false;
    }
    public void Toggle()
    {
        if (Show)
        {
            HidePanel();
        }
        else
        {
            ShowPanel();
        }
    }
    public void SwapItemPanels(ItemPanel from, ItemPanel to)
    {
        Equippable[] items = new Equippable[2];
        items[0] = from.obj;
        items[1] = to.obj;
        from.Display(items[1]);
        to.Display(items[0]);
    }
    #endregion
    #region Private
    void MovePanel(InventoryPanel obj, string pos, Tweener t)
    {
        Panel.Position target = obj.panel[pos];
        if (obj.panel.CurrentPosition != target)
        {
            if (t != null && t.easingControl != null)
                t.easingControl.Stop();
            t = obj.panel.SetPosition(pos, true);
            t.easingControl.duration = 0.5f;
            t.easingControl.equation = EasingEquations.EaseOutQuad;
        }
    }

    void ItemPanelUpdate(object sender, object args)
    {
        ItemPanel guess = (ItemPanel)args;
        float closestDistance = 16;
        ItemPanel target = guess;
        for (int i = 0; i < itemPositions.Length; i++)
        {
            Debug.Log(closestDistance);
            Debug.Log(Vector3.Distance(guess.transform.position, itemPositions[i]));
            if (Vector3.Distance(guess.transform.position, itemPositions[i]) < closestDistance)
            {
                target = panel.items[i];
                closestDistance = Vector3.Distance(guess.transform.position, itemPositions[i]);
            }
        }

        Debug.Log(target);
        Debug.Log(guess);
        if (target == guess)
            return;
        SwapItemPanels(guess, target);
    }

    void CheckLayout(object sender, object args)
    {
        panel.panel.RemovePosition(ShowKey);
        panel.panel.RemovePosition(HideKey);
        panel.panel.AddPosition(ShowKey, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, Vector2.zero);
        panel.panel.AddPosition(HideKey, TextAnchor.UpperCenter, TextAnchor.LowerCenter, Vector2.zero);

        panel.panel.SetPosition(ShowKey, false);

        itemPositions = new Vector3[panel.items.Count];

        for (int i = 0; i < panel.items.Count; i++)
        {
            itemPositions[i] = panel.items[i].transform.position;
        }

        if (Show)
            panel.panel.SetPosition(ShowKey, false);
        else
            panel.panel.SetPosition(HideKey, false);
    }
    #endregion
}