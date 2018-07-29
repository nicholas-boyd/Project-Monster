using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatPanelController : MonoBehaviour
{
    #region Const
    const string ShowKey = "Show";
    const string HideKey = "Hide";
    #endregion
    #region Fields
    [SerializeField] List<StatPanel> panels;
    /*[SerializeField] StatPanel primaryPanel;
    [SerializeField] StatPanel secondaryPanel;*/

    List<Tweener> tweeners;
    /*Tweener primaryTransition;
    Tweener secondaryTransition;*/
    public int Count { get { return panels.Count; } }
    #endregion
    #region MonoBehaviour
    void Start()
    {
        tweeners = new List<Tweener>();
        for (int i = 0; i < panels.Count; i++)
        {
            if (panels[i].panel.CurrentPosition == null)
            {
                panels[i].panel.SetPosition(HideKey, false);
            }
            while (tweeners.Count != Count)
                tweeners.Add(panels[i].panel.SetPosition(ShowKey, true));
            tweeners[i] = panels[i].panel.SetPosition(HideKey, true);
            tweeners[i].easingControl.duration = 0.5f;
            tweeners[i].easingControl.equation = EasingEquations.EaseOutQuad;
        }
        /*
        if (primaryPanel.panel.CurrentPosition == null)
            primaryPanel.panel.SetPosition(HideKey, false);
        if (secondaryPanel.panel.CurrentPosition == null)
            secondaryPanel.panel.SetPosition(HideKey, false);*/
    }
    #endregion
    #region Public
    public void ShowInPanel(GameObject obj, int index)
    {
        panels[index].Display(obj);
        MovePanel(panels[index], ShowKey, tweeners[index]);
    }
    public void HidePanel(int index)
    {
        MovePanel(panels[index], HideKey, tweeners[index]);
    }
    #endregion
    #region Private
    void MovePanel(StatPanel obj, string pos, Tweener t)
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
    #endregion
}