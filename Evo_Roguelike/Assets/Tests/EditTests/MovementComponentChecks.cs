using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementComponentChecks
{
    // Test that the set wander position is within the bounds of the screen
    [Test]
    public void TestSetWanderPosition()
    {
        // Get the bounds of the screen
        float maxHeight = Camera.main.GetComponent<Camera>().orthographicSize;
        float maxWidth = maxHeight * (Screen.width / Screen.height);

        // Create an instance of the movement component
        GameObject testObj = new GameObject();
        KinematicMovement movementControls = testObj.AddComponent<KinematicMovement>();

        // Create a sprite renderer component and set reference in movement controls
        // without this the test fails due to sprite renderer being null in this test scenario by default
        SpriteRenderer spriteRender = testObj.AddComponent<SpriteRenderer>();
        spriteRender.size = new Vector2(1.0f, 1.0f);
        movementControls.SpriteRenderer = spriteRender;

        // Set a new wander position
        movementControls.SetWanderPosition();

        // Assert the position is within the bounds of the screen
        Assert.Greater(movementControls.targetPosition.x, -maxWidth * 2);
        Assert.Greater(movementControls.targetPosition.y, -maxHeight);
        Assert.Less(movementControls.targetPosition.x, maxWidth * 2);
        Assert.Less(movementControls.targetPosition.y, maxHeight);
    }
}
