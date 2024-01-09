using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * Responsible for the level editor window and its functionalities.
 */
public class LevelEditor : EditorWindow
{

    public GridManager gridManager;

    // Serialized objects and properties
    public SerializedObject serializedObject;
    public SerializedProperty propGridManager;

    /*
     * Creates window from menu at "Tools/LevelEditor"
     */
    [MenuItem ("Tools/LevelEditor")]
    public static void OpenLevelEditor() => GetWindow<LevelEditor>();


    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        propGridManager = serializedObject.FindProperty("gridManager");
    }
  
    private void OnGUI()
    {
        /*
         * This function is called by the Unity Editor to specify what to display in the editor window.
         */

        // Syncing serialized object with target object
        serializedObject.Update();

        EditorGUILayout.PropertyField(propGridManager);
        if (GUILayout.Button("Generate New Island"))
        {
            if(gridManager != null )
            {
                // This is temporary input data. Will be replaced with data from PCG in the future.
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

        // Flushing any changes made.
        if(serializedObject.ApplyModifiedProperties())
        {
            Debug.Log("Level Editor property change");
        }
    }
}
