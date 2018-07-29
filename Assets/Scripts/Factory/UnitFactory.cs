using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class UnitFactory
{
    public static GameObject Create(string name)
    {
        UnitRecipe recipe = Resources.Load<UnitRecipe>("Recipes/UnitRecipes/" + name);
        if (recipe == null)
        {
            Debug.LogError("No Unit Recipe for name: " + name);
            return null;
        }
        return Create(recipe);
    }
    public static GameObject Create(UnitRecipe recipe)
    {
        GameObject obj = InstantiatePrefab("Prefabs/Units/Models/" + recipe.model);
        obj.name = recipe.name;
        obj.AddComponent<Unit>();
        AddStats(obj);
        AddMovementType(obj, recipe.movement);
        obj.AddComponent<Status>();
        AddJob(obj, recipe.job);
        obj.AddComponent<Health>();
        obj.AddComponent<Mana>();
        AddDeck(obj, recipe.deckRecipe);
        AddAlliance(obj, recipe.alliance);
        AddHitPanel(obj);
        if (recipe.job != "Hero")
        {
            AddAttackPattern(obj, recipe.strategy);
            obj.AddComponent<AIPlayerController>();
        }

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
    static void AddStats(GameObject obj)
    {
        obj.AddComponent<Stats>();
    }
    static void AddJob(GameObject obj, string name)
    {
        GameObject instance = InstantiatePrefab("Prefabs/Jobs/" + name);
        instance.transform.SetParent(obj.transform);
        Job job = instance.GetComponent<Job>();
        job.Employ();
        job.LoadDefaultStats();
    }
    static void AddAttackPattern(GameObject obj, string name)
    {
        Driver driver = obj.AddComponent<Driver>();
        if (string.IsNullOrEmpty(name))
        {
            driver.normal = Drivers.Human;
        }
        else
        {
            driver.normal = Drivers.Computer;
            GameObject instance = InstantiatePrefab("Prefabs/AttackPatterns/" + name);
            instance.transform.SetParent(obj.transform);
        }
    }
    static void AddMovementType(GameObject obj, MovementType type)
    {
        switch (type)
        {
            case MovementType.WalkMovement:
                obj.AddComponent<WalkMovement>();
                obj.GetComponent<Movement>().range = 1;
                break;
            case MovementType.EnemyWalkMovement:
                Movement move = obj.AddComponent<EnemyWalkMovement>();
                move.range = 4;
                break;
        }
    }
    static void AddAlliance(GameObject obj, Alliances type)
    {
        Alliance alliance = obj.AddComponent<Alliance>();
        alliance.type = type;
    }
    static void AddDeck(GameObject obj, string name)
    {
        List<Card> deck = new List<Card>();
        if (name == "")
        {
            obj.GetComponent<Unit>().deck = new AbilityDeck(new List<Card>());
            return;
        }
        DeckRecipe recipe = Resources.Load<DeckRecipe>("Recipes/DeckRecipes/" + name);
        if (recipe == null)
        {
            Debug.LogError("No Deck Recipe Found: " + name);
            return;
        }
        for (int i = 0; i < recipe.CardNames.Length; ++i)
        {
            CardRecipe data = Resources.Load("Recipes/CardRecipes/" + recipe.CardNames[i]) as CardRecipe;
            if (data == null)
            {
                Debug.LogError("No CardData for name: " + recipe.CardNames[i]);
                continue;
            }
            GameObject prefab = new GameObject(recipe.CardNames[i]);
            Card card = prefab.AddComponent<Card>();
            card.transform.parent = obj.transform;
            card.Load(data);
            deck.Add(card);
        }
        obj.GetComponent<Unit>().deck = new AbilityDeck(deck);
        obj.GetComponent<Unit>().deck.Shuffle();
        obj.GetComponent<Unit>().discards = new AbilityDeck(new List<Card>());
    }

    static void AddHitPanel(GameObject obj)
    {
        HitPanel hp = InstantiatePrefab("Prefabs/Units/HitPanel").GetComponent<HitPanel>();
        hp.transform.SetParent(obj.transform);
    }
}