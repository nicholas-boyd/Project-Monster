using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuState : GameState {

    public override void Enter()
    {
        base.Enter();
        SpawnTestItems();
        owner.gameMenuController.Show();
        StartCoroutine(GameMenu());
    }

    IEnumerator GameMenu()
    {
        while (!owner.gameMenuController.hidden)
        {
            yield return null;
        }
        owner.battleController.Activate();
    }

    void SpawnTestItems()
    {
        string[] items = new string[]
        {
            "Helmet",
            "Upper",
            "Lower",
            "Gloves",
            "Boots"
        };
        for (int i = 0; i < items.Length; i++)
        {
            GameObject instance = ItemFactory.Create(items[i]);
            instance.transform.SetParent(owner.transform.Find("Trackable Items"));
            Equippable eq = instance.GetComponent<Equippable>();
            owner.gameMenuController.inventoryPanelController.ShowInPanel(eq, i, false);
        }
    }
}
