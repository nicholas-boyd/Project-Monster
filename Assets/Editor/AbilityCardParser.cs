using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
public static class AbilityCardParser
{
    const int DefaultManaCost = 10;
    const int DefaultPower = 20;
    const int DefaultRange = 1;

    [MenuItem("Pre Production/Parse AbilityCards")]
    public static void Parse()
    {
        CreateDirectories();
        ParseAbilities();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void CreateDirectories()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Prefabs/AbilityCards"))
            AssetDatabase.CreateFolder("Assets/Resources/Prefabs", "AbilityCards");
    }

    static void ParseAbilities()
    {
        string readPath = string.Format("{0}/Settings/AbilityCards.csv", Application.dataPath);
        string[] readText = File.ReadAllLines(readPath);
        for (int i = 1; i < readText.Length; ++i)
            ParseAbilities(readText[i]);
    }

    static void ParseAbilities(string line)
    {
        string[] elements = line.Split(',');
        CardRecipe recipe = GetOrCreateRecipe(elements[0]);
        

        recipe.Name = elements[0];
        recipe.Description = elements[1];

        string spritePath = string.Format("Sprites/Abilities/{0}", elements[2]);
        string rangeSpritePath = string.Format("Sprites/RangeIndicators/{0}", elements[3]);
        string particleEffectPath = string.Format("Prefabs/ParticleEffects/FinalizedEffects/{0}", elements[4]);
        string redHitPath = "Prefabs/ParticleEffects/FinalizedEffects/particle_hitred";
        string blueHitPath = "Prefabs/ParticleEffects/FinalizedEffects/particle_hitblue";
        string defaultSpritePath = "Sprites/Abilities/ability_default";
        string defaultRangeSpritePath = "Sprites/RangeIndicators/range_default";
        string defaultParticleEffectPath = "Prefabs/ParticleEffects/FinalizedEffects/particle_default";

        recipe.Sprite = Resources.Load<Sprite>(spritePath);
        recipe.AbilityRangeIndicator = Resources.Load<Sprite>(rangeSpritePath);
        recipe.ParticleEffect = Resources.Load<ParticleSystem>(particleEffectPath);
        recipe.RedHitMarker = Resources.Load<ParticleSystem>(redHitPath);
        recipe.BlueHitMarker = Resources.Load<ParticleSystem>(blueHitPath);
        Debug.Log(recipe.ParticleEffect);
        Debug.Log(recipe.RedHitMarker);
        Debug.Log(recipe.BlueHitMarker);

        switch (elements[5])
        {
            case "single":
                recipe.ParticalEffectType = ParticleEffectType.Single;
                break;
            case "hybrid":
                recipe.ParticalEffectType = ParticleEffectType.Hybrid;
                break;
            case "multiple":
            default:
                recipe.ParticalEffectType = ParticleEffectType.Multiple;
                break;
        }

        int manaCost = DefaultManaCost;
        int power = DefaultPower;
        int range = DefaultRange;
        recipe.ProjectileEffect = elements[6].Equals("TRUE") ? true : false;
        Int32.TryParse(elements[7], out manaCost);
        Int32.TryParse(elements[11], out power);
        Int32.TryParse(elements[13], out range);
        recipe.AbilityManaCost = manaCost;
        recipe.BasePower = power;
        recipe.AbilityRangeHorizontal = range;

        string abilityEffect = string.Format("{0}AbilityEffect", elements[8]);
        string abilityEffectTarget = string.Format("{0}AbilityEffectTarget", elements[9]);
        string abilityPower = string.Format("{0}AbilityPower", elements[10]);
        string abilityRange = string.Format("{0}AbilityRange", elements[12]);
        recipe.AbilityEffect = abilityEffect;
        recipe.AbilityEffectTarget = abilityEffectTarget;
        recipe.AbilityPower = abilityPower;
        recipe.AbilityRange = abilityRange;


        GameObject obj = GetOrCreateCard(elements[0]);
        Card card = obj.GetComponent<Card>();
        card.Load(recipe);

        if (recipe.AbilityEffect.Contains("InflictStatus"))
            Debug.Log(recipe.AbilityEffect);

        // Useful, but annoying when debugging this script
        if (recipe.Sprite == null)
        {
            Debug.Log(string.Format("Couldn't load {0} card sprite, titled \"{1}\", at \"{2}\". Setting default.", elements[0], elements[2], spritePath));
            recipe.Sprite = Resources.Load<Sprite>(defaultSpritePath);
        }
        if (recipe.AbilityRangeIndicator == null)
        {
            Debug.Log(string.Format("Couldn't load {0} card range indicator, titled \"{1}\", at \"{2}\". Setting default.", elements[0], elements[3], rangeSpritePath));
            recipe.AbilityRangeIndicator = Resources.Load<Sprite>(defaultRangeSpritePath);
        }
        Debug.Log(recipe.ParticleEffect);
        if (recipe.ParticleEffect == null)
        {
            Debug.Log(string.Format("Couldn't load {0} card particle effect, titled \"{1}\", at \"{2}\". Setting default.", elements[0], elements[4], particleEffectPath));
            recipe.ParticleEffect = Resources.Load<ParticleSystem>(defaultParticleEffectPath);
        }
    }

    static StatModifierFeature GetFeature(GameObject obj, StatTypes type)
    {
        StatModifierFeature[] smf = obj.GetComponents<StatModifierFeature>();
        for (int i = 0; i < smf.Length; ++i)
        {
            if (smf[i].type == type)
                return smf[i];
        }
        StatModifierFeature feature = obj.AddComponent<StatModifierFeature>();
        feature.type = type;
        return feature;
    }

    static GameObject GetOrCreateCard(string abilityName)
    {
        string fullPath = string.Format("Assets/Resources/Prefabs/AbilityCards/{0}.prefab", abilityName);
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
        if (obj == null)
            obj = Create(fullPath);
        EditorUtility.SetDirty(obj);
        return obj;
    }

    static CardRecipe GetOrCreateRecipe(string abilityName)
    {
        string fullPath = string.Format("Assets/Resources/Recipes/CardRecipes/{0}.asset", abilityName);
        CardRecipe obj = AssetDatabase.LoadAssetAtPath<CardRecipe>(fullPath);
        if (obj == null)
            obj = CreateRecipe(fullPath);
        EditorUtility.SetDirty(obj);
        return obj;
    }

    static GameObject Create(string fullPath)
    {
        GameObject instance = new GameObject("temp");
        instance.AddComponent<Card>();
        GameObject prefab = PrefabUtility.CreatePrefab(fullPath, instance);
        GameObject.DestroyImmediate(instance);
        return prefab;
    }

    static CardRecipe CreateRecipe(string fullPath)
    {
        CardRecipe instance = ScriptableObject.CreateInstance<CardRecipe>();
        AssetDatabase.CreateAsset(instance, fullPath);
        CardRecipe prefab = AssetDatabase.LoadAssetAtPath<CardRecipe>(fullPath);
        if (prefab == null)
            Debug.Log("Recipe Creation Failed");
        return prefab;
    }
}