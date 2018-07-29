using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPanel : MonoBehaviour {

    public Image background;
    public List<Sprite> sprites;
    void Start()
    {
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    private void OnEnable()
    {
        this.AddObserver(Transition, MainMenuController.TransitionNotification);
        this.AddObserver(Transition, GameMenuController.TransitionNotification);
    }

    private void OnDisable()
    {
        this.RemoveObserver(Transition, MainMenuController.TransitionNotification);
        this.RemoveObserver(Transition, GameMenuController.TransitionNotification);
    }

    public void Transition(object sender, object args)
    {
        if ((bool)args)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Show()
    {
        background.CrossFadeAlpha(1, 0f, true);
    }

    public void Hide()
    {
        background.CrossFadeAlpha(0, 0f, true);
    }
}
