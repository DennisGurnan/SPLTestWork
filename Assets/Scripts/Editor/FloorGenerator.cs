using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FloorGenerator : EditorWindow
{
    private GameObject tilePrefab;
    private GameObject parent;
    [MenuItem("SPL/Floor Generator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FloorGenerator));
    }

    private void OnGUI()
    {
        tilePrefab = EditorGUILayout.ObjectField("Tile prefab", tilePrefab, typeof(GameObject), true) as GameObject;
        parent = EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject), true) as GameObject;
        GUI.enabled = tilePrefab != null && parent != null;
        if (GUILayout.Button("Generate"))
        {
            GenerateFloor();
        }
    }

    private void GenerateFloor()
    {
        for(int i = -2; i < 4; i += 2)
        {
            for(int j = 0; j < 52; j += 2)
            {
                GameObject tile = PrefabUtility.InstantiatePrefab(tilePrefab as GameObject) as GameObject;
                tile.transform.parent = parent.transform;
                tile.transform.localPosition = new Vector3(i, -0.1f, j); 
            }
        }
    }
}
