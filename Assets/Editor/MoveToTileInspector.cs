using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoveToTile))]
public class MoveToTileInspector : Editor {

    public MoveToTile current
    {
        get
        {
            return (MoveToTile)target;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (current.Object && current.Tile)
        {
            current.Object.transform.position = current.Tile.center;
        }
    }
}
