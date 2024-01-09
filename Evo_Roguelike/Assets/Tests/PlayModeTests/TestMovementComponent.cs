using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class TestMovementComponent
{
    // Load scene to test
    [OneTimeSetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene("CreatureMovementTesting");
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestMovementComponentWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        
        
        yield return null;
    }
}
