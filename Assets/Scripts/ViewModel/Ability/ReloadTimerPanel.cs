using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReloadTimerPanel : MonoBehaviour {

    [SerializeField] Text timer;
    public Panel panel;
    public Image background;

    public void Start()
    {
        background.color = Color.clear;
    }

    public void Display(float time)
    {
        background.color = Color.gray;
        timer.text = time.ToString("F2");
    }
}
