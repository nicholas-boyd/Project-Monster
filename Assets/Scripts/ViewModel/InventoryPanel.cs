using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryPanel : MonoBehaviour {

    public Panel panel;
    public List<EquippedItemPanel> equips;
    public List<ItemPanel> items;
    public Image background;
    public Image icon;
    public Text nameLabel;
    public void DisplayItem(Equippable obj, int index)
    {
        ItemPanel itemPanel = items[index];
        if (itemPanel)
            itemPanel.Display(obj);
    }

    public void DisplayEquip(Equippable obj, int index)
    {
        EquippedItemPanel itemPanel = equips[index];
        if (itemPanel)
            itemPanel.Display(obj);
    }
}
