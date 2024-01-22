using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * Responsible for the level editor window and its functionalities.
 */
public class LevelEditor : EditorWindow
{

    private GridManager _gridManager;

    // Serialized objects and properties
    public SerializedObject serializedObject;


    public GridManager gridManager
    {
        get
        {
            if (_gridManager == null)
            {
                GridManagerBehaviour gridManagerBehaviour = GameObject.FindObjectOfType<GridManagerBehaviour>();
                _gridManager = gridManagerBehaviour.gridManager;
            }
            return _gridManager; 
        }
    }

    /*
     * Creates window from menu at "Tools/LevelEditor"
     */
    [MenuItem ("Tools/LevelEditor")]
    public static void OpenLevelEditor() => GetWindow<LevelEditor>();


    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);

        if(ServiceLocator.Instance == null)
        {
            Debug.Log("No service locator");
        }
    }

    private void OnGUI()
    {
        /*
         * This function is called by the Unity Editor to specify what to display in the editor window.
         */

        // Syncing serialized object with target object
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();

        //EditorGUILayout.PropertyField(propGridManager);
        if (GUILayout.Button("Generate New Island") && ShowConfirmationBox("Display New Island?"))
        {
            gridManager.GenerateTileData();        
        }

        if(GUILayout.Button("Clear Tiles") && ShowConfirmationBox("Clear All Tiles?"))
        {
            gridManager.ClearTilemap();
        }

        EditorGUILayout.EndHorizontal();

        // Flushing any changes made.
        if(serializedObject.ApplyModifiedProperties())
        {
            Debug.Log("Level Editor property change");
        }
    }

    private bool ShowConfirmationBox(string message)
    {
        return EditorUtility.DisplayDialog("Please Confirm", message, "Ok", "Cancel");
    }
}
