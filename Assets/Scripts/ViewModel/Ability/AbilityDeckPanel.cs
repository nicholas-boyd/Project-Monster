using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AbilityDeckPanel : MonoBehaviour
{
    [SerializeField] public List<Sprite> sprites;
    public Image activeSprite;
    public int maxSize = 1;
    public Panel panel;
    private void Start()
    {
        activeSprite.color = Color.clear;
        if (GetComponentInChildren<ReloadTimerPanel>() != null)
        {
            GetComponentInChildren<ReloadTimerPanel>().background.color = Color.clear;
        }
    }
    public void Display(int size)
    {
        activeSprite.color = Color.white;
        if (size > maxSize)
            maxSize = size;
        int spriteIndex = Mathf.Clamp(Mathf.RoundToInt(size / maxSize * sprites.Count), 0, sprites.Count - 1);
        if (size > 0 && spriteIndex == 0)
            spriteIndex = 1;
        activeSprite.sprite = sprites[spriteIndex];
    }
}
