using UnityEngine;
using UnityEditor;
public class YourClassAsset
{
    [MenuItem("Assets/Create/Unit Recipe")]
    public static void CreateUnitRecipe()
    {
        ScriptableObjectUtility.CreateAsset<UnitRecipe>();
    }
    [MenuItem("Assets/Create/Deck Recipe")]
    public static void CreateDeckRecipe()
    {
        ScriptableObjectUtility.CreateAsset<DeckRecipe>();
    }

    [MenuItem("Assets/Create/Card Recipe")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<CardRecipe>();
    }
}