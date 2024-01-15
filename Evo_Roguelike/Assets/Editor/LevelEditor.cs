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

        EditorGUILayout.PropertyField(propGridManager);
        if (GUILayout.Button("Generate New Island"))
        {
            if(gridManager != null )
            {

                gridManager.GenerateTileData();
            }
        }

        // Flushing any changes made.
        if(serializedObject.ApplyModifiedProperties())
        {
            Debug.Log("Level Editor property change");
        }
    }
}
