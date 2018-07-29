using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
public static class JobParser {
    [MenuItem("Pre Production/Parse Jobs")]
    public static void Parse() {
        CreateDirectories();
        ParseStartingStats();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void CreateDirectories() {
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Prefabs/Jobs"))
            AssetDatabase.CreateFolder("Assets/Resources/Prefabs", "Jobs");
    }

    static void ParseStartingStats() {
        string readPath = string.Format("{0}/Settings/JobStartingStats.csv", Application.dataPath);
        string[] readText = File.ReadAllLines(readPath);
        for (int i = 1; i < readText.Length; ++i)
            ParseStartingStats(readText[i]);
    }

    static void ParseStartingStats(string line) {
        string[] elements = line.Split(',');
        GameObject obj = GetOrCreate(elements[0]);
        Job job = obj.GetComponent<Job>();
        for (int i = 1; i < Job.statOrder.Length + 1; ++i)
            job.baseStats[i - 1] = Convert.ToInt32(elements[i]);
        
    }

    // UNUSED
    /*
    static StatModifierFeature GetFeature(GameObject obj, StatTypes type) {
        StatModifierFeature[] smf = obj.GetComponents<StatModifierFeature>();
        for (int i = 0; i < smf.Length; ++i) {
            if (smf[i].type == type)
                return smf[i];
        }
        StatModifierFeature feature = obj.AddComponent<StatModifierFeature>();
        feature.type = type;
        return feature;
    }
    */

    static GameObject GetOrCreate(string jobName) {
        string fullPath = string.Format("Assets/Resources/Prefabs/Jobs/{0}.prefab", jobName);
        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
        if (obj == null)
            obj = Create(fullPath);
        EditorUtility.SetDirty(obj);
        return obj;
    }

    static GameObject Create(string fullPath) {
        GameObject instance = new GameObject("temp");
        instance.AddComponent<Job>();
        GameObject prefab = PrefabUtility.CreatePrefab(fullPath, instance);
        GameObject.DestroyImmediate(instance);
        return prefab;
    }
}