using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryPanel : MonoBehaviour {

    public Panel panel;
    public List<ItemPanel> items;
    public Image background;
    public Image icon;
    public Text nameLabel;
    public void Display(Equippable obj, int index)
    {
        ItemPanel itemPanel = items[index];
        itemPanel.Display(obj);
    }
}
