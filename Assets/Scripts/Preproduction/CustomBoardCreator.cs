
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CustomBoardCreator : MonoBehaviour
{

    [SerializeField] List<GameObject> tilePrefabs;

    Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();

    [SerializeField] int width = 6;
    [SerializeField] int depth = 3;

    [SerializeField] LevelData levelData;

    Tile Create(int x)
    {
        GameObject instance = Instantiate(tilePrefabs[x]) as GameObject;
        instance.transform.parent = transform;
        return instance.GetComponent<Tile>();
    }

    Tile GetOrCreate(Point p)
    {
        if (tiles.ContainsKey(p))
            return tiles[p];

        Tile t = p.x < width / 2 ? Create(0) : Create(1);
        t.Load(p);
        tiles.Add(p, t);

        return t;
    }

    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
            DestroyImmediate(transform.GetChild(i).gameObject);
        tiles.Clear();
    }

    public void Save()
    {
        string filePath = Application.dataPath + "/Resources/Levels";
        if (!Directory.Exists(filePath))
            CreateSaveDirectory();

        LevelData board = ScriptableObject.CreateInstance<LevelData>();
        board.tiles = new List<Point>(tiles.Count);
        board.tiles.Add(new Point(width, depth));
        foreach (Tile t in tiles.Values)
            board.tiles.Add(new Point(t.pos.x, t.pos.y));

        string fileName = string.Format("Assets/Resources/Levels/{1}.asset", filePath, name);
        AssetDatabase.CreateAsset(board, fileName);
    }

    public void SetBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                GetOrCreate(new Point(i, j));
            }
        }
    }

    void CreateSaveDirectory()
    {
        string filePath = Application.dataPath + "/Resources";
        if (!Directory.Exists(filePath))
            AssetDatabase.CreateFolder("Assets", "Resources");
        filePath += "/Levels";
        if (!Directory.Exists(filePath))
            AssetDatabase.CreateFolder("Assets/Resources", "Levels");
        AssetDatabase.Refresh();
    }

    public void Load()
    {
        Clear();
        if (levelData == null)
            return;
        foreach (Point p in levelData.tiles)
        {
            if (p == levelData.tiles[0])
            {
                width = p.x;
                depth = p.y;
            }
            else
            {
                Tile t = p.x < width / 2 ? Create(0) : Create(1);
                t.Load(p);
                tiles.Add(t.pos, t);
            }
        }
    }


}
#endif
