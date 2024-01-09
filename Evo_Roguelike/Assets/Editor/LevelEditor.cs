using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditor : EditorWindow
{

    public GridManager gridManager;

    public SerializedObject serializedObject;
    public SerializedProperty propGridManager;

    // Creates editor window from menu
    [MenuItem ("Tools/LevelEditor")]
    public static void OpenLevelEditor() => GetWindow<LevelEditor>();

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        propGridManager = serializedObject.FindProperty("gridManager");
    }

    private void OnGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(propGridManager);
        if (GUILayout.Button("Generate New Island"))
        {
            //GridManager gridManager = ServiceLocator.Instance.GetService<GridManager>();
            if(gridManager != null )
            {
                int[,] testTiles = { 
                                   { 0, 1, 1, 1, 0, 0, 2, 2, 2, 0, 0, 5, 0, 5, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                                   { 0, 1, 0, 1, 0, 0, 2, 0, 0, 0, 0, 5, 5, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                                   { 0, 1, 1, 1, 0, 0, 2, 2, 2, 0, 0, 5, 5, 5, 0, 0, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0 },
                                   { 0, 0, 0, 1, 0, 0, 2, 0, 0, 0, 0, 5, 0, 5, 0, 0, 6, 0, 6, 0, 0, 0, 0, 0, 0, 0 },
                                   { 0, 0, 0, 1, 0, 0, 2, 2, 2, 0, 0, 5, 5, 5, 0, 0, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0 }
                                   };


                gridManager.GenerateGroundTilesEditor(testTiles);
            }
        }

        if(serializedObject.ApplyModifiedProperties())
        {
            Debug.Log("Level Editor property change");

        }
    }
}
