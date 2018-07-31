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
    public Vector3[] equipPositions;
    [SerializeField] InventoryPanel panel;
    public bool Show = false;

    Tweener tweener;
    #endregion
    #region MonoBehaviour
    void Start()
    {
        panel.panel.SetPosition(ShowKey, false);
        
        itemPositions = new Vector3[panel.items.Count];
        equipPositions = new Vector3[panel.equips.Count];


        for (int i = 0; i < panel.items.Count; i++)
        {
            itemPositions[i] = panel.items[i].transform.position;
        }

        for (int i = 0; i < panel.equips.Count; i++)
        {
            equipPositions[i] = panel.equips[i].transform.position;
        }

        panel.panel.SetPosition(HideKey, false);
    }

    void OnEnable()
    {
        this.AddObserver(ItemPanelUpdate, ItemPanel.ItemPanelDragNotification);
        this.AddObserver(ItemPanelUpdate, EquippedItemPanel.ItemPanelDragNotification);
        this.AddObserver(CheckLayout, InputController.ScreenSizeUpdateNotification);
    }

    void OnDisable()
    {
        this.RemoveObserver(ItemPanelUpdate, ItemPanel.ItemPanelDragNotification);
        this.RemoveObserver(ItemPanelUpdate, EquippedItemPanel.ItemPanelDragNotification);
        this.RemoveObserver(CheckLayout, InputController.ScreenSizeUpdateNotification);
    }
    #endregion
    #region Public
    public void ShowInPanel(Equippable obj, int index, bool equip)
    {
        if (equip)
            panel.DisplayEquip(obj, index);
        else
            panel.DisplayItem(obj, index);
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
    public void SwapItemPanels(GameObject from, GameObject to)
    {
        ItemPanel panel = from.GetComponent<ItemPanel>();
        ItemPanel otherPanel = to.GetComponent<ItemPanel>();
        if (panel && otherPanel)
        {
            Equippable[] items = new Equippable[2];
            items[0] = panel.obj;
            items[1] = otherPanel.obj;
            panel.Display(items[1]);
            otherPanel.Display(items[0]);
            Debug.Log("Display swap");
            return;
        }

        EquippedItemPanel equipPanel = from.GetComponent<EquippedItemPanel>();
        if (!equipPanel)
        {
            equipPanel = to.GetComponent<EquippedItemPanel>();
            otherPanel = from.GetComponent<ItemPanel>();
        }
        if (equipPanel.equipSlots == otherPanel.obj.defaultSlots || equipPanel.equipSlots == otherPanel.obj.secondarySlots)
        {
            Equippable[] items = new Equippable[2];
            items[0] = equipPanel.obj;
            items[1] = otherPanel.obj;
            equipPanel.Display(items[1]);
            otherPanel.Display(items[0]);
            Debug.Log("Display swap");
            return;
        }
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
        GameObject guess = (GameObject)args;
        float closestDistance = 16;
        GameObject target = guess;
        for (int i = 0; i < itemPositions.Length; i++)
        {
            Debug.Log(closestDistance);
            Debug.Log(Vector3.Distance(guess.transform.position, itemPositions[i]));
            if (Vector3.Distance(guess.transform.position, itemPositions[i]) < closestDistance)
            {
                target = panel.items[i].gameObject;
                closestDistance = Vector3.Distance(guess.transform.position, itemPositions[i]);
            }
        }

        for (int i = 0; i < equipPositions.Length; i++)
        {
            Debug.Log(closestDistance);
            Debug.Log(Vector3.Distance(guess.transform.position, equipPositions[i]));
            if (Vector3.Distance(guess.transform.position, equipPositions[i]) < closestDistance)
            {
                target = panel.equips[i].gameObject;
                closestDistance = Vector3.Distance(guess.transform.position, equipPositions[i]);
            }
        }
        if (target == guess)
            return;
        SwapItemPanels(guess.gameObject, target);
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