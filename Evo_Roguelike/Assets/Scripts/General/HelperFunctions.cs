using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class HelperFunctions
{
    public static bool IsMouseWithinScreen()
    {
        /*
         * Checks if mouse is within viewport/screen
         */

        if (Mouse.current == null) return false;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        if (mousePos.x > 0 && mousePos.y > 0 && mousePos.x < Screen.width && mousePos.y < Screen.height) { return true; }

        return false;
    }
}