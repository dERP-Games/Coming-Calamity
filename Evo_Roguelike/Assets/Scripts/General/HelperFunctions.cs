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

    /// <summary>
    /// Generates Gaussian Values
    /// </summary>
    /// <param name="mean">Average value in distribution</param>
    /// <param name="stdDev">Standard deviation in distribution</param>
    /// <returns>float, random value on gaussian distribution</returns>
    public static float Gaussian(float mean, float stdDev)
    {
        float val1 = Random.Range(0f, 1f);
        float val2 = Random.Range(0f, 1f);
        float gaussValue =
            Mathf.Sqrt(-2.0f * Mathf.Log(val1)) *
            Mathf.Sin(2.0f * Mathf.PI * val2);
        return mean + stdDev * gaussValue;
    }
}
