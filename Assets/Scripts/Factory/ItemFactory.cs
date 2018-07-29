using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class ItemFactory
{
    public static GameObject Create(string name)
    {
        ItemRecipe recipe = Resources.Load<ItemRecipe>("Recipes/ItemRecipes/" + name);
        if (recipe == null)
        {
            Debug.LogError("No Unit Recipe for name: " + name);
            return null;
        }
        return Create(recipe);
    }
    public static GameObject Create(ItemRecipe recipe)
    {
        GameObject obj = InstantiatePrefab("Prefabs/Equipment/" + recipe.Name);
        obj.name = recipe.name;
        obj.AddComponent<Equippable>();
        Equippable item = obj.GetComponent<Equippable>();
        item.defaultSlots = recipe.defaultSlots;
        item.secondarySlots = recipe.secondarySlots;
        return obj;
    }
    static GameObject InstantiatePrefab(string name)
    {
        GameObject prefab = Resources.Load<GameObject>(name);
        if (prefab == null)
        {
            Debug.LogError("No Prefab for name: " + name);
            return new GameObject(name);
        }
        GameObject instance = GameObject.Instantiate(prefab);
        return instance;
    }
}