using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class for handling in-game UI
/// </summary>
public class UIManager : MonoBehaviour
{
    // Public fields
    public GameObject TraitPanelRef;


    /// <summary>
    /// Used for enabling trait GUI panel
    /// </summary>
    public void EnableTraitsGUI()
    {
        TraitPanelRef.SetActive(true);
    }
}
