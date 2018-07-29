using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomBoardCreator))]
public class BoardCreatorInspector : Editor {

	public CustomBoardCreator current {
		get {
			return (CustomBoardCreator)target;
		}
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();
		if (GUILayout.Button("Set Board"))
			current.SetBoard();
		if (GUILayout.Button("Clear"))
			current.Clear();
		if (GUILayout.Button("Save"))
			current.Save();
		if (GUILayout.Button("Load"))
			current.Load();
	}
}
