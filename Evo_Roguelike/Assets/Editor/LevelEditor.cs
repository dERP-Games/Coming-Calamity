using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
 * Responsible for the level editor window and its functionalities.
 */
public class LevelEditor : EditorWindow
{

    private GridManagerBehaviour _gridManagerBehaviour;

    // Serialized objects and properties
    public SerializedObject serializedObject;


    public GridManagerBehaviour gridManagerBehaviour
    {
        get
        {
            if (_gridManagerBehaviour == null)
            {
                _gridManagerBehaviour = GameObject.FindObjectOfType<GridManagerBehaviour>();
            }
            return _gridManagerBehaviour; 
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

        if(GUILayout.Button("Initialize Level Editor"))
        {
            InitializeVitalComponents();
        }

        


        //EditorGUILayout.BeginHorizontal();

        //EditorGUILayout.PropertyField(propGridManager);
        if (GUILayout.Button("Generate New Island") && ShowConfirmationBox("Display New Island?"))
        {
            if(gridManagerBehaviour == null)
            {
                Debug.Log("No grid manager");
            }
            else
            {
                gridManagerBehaviour.GridManager.GenerateTileData();
            }       
        }

        if (GUILayout.Button("Clear Tiles") && ShowConfirmationBox("Clear All Tiles?"))
        {
            if (gridManagerBehaviour == null)
            {
                Debug.Log("No grid manager");
            }
            else
            {
                gridManagerBehaviour.GridManager.ClearTilemap();
            }
        }

       // EditorGUILayout.EndHorizontal(); 
       

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

    private void InitializeVitalComponents()
    {
        if (_gridManagerBehaviour == null)
        {
            _gridManagerBehaviour = GameObject.FindObjectOfType<GridManagerBehaviour>();
        }
    }

    private bool AreVitalComponentsInitialized()
    {
        if(_gridManagerBehaviour == null)
        {
            return false;
        }
        return true;
    }
}
