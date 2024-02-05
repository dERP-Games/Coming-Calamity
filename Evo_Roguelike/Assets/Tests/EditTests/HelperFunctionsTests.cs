using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

public class HelperFunctionsTests
{
    // Test ReLU helper function
    [Test]
    public void TestZeroOrMore()
    {
        float negativeCase = -1.0f;
        float zeroCase = 0.0f;
        float positiveCase = 1.0f;
        float expectedOutputNegative = 0;
        float expectedOutputZero = 0.0f;
        float expectedOutputPositive = 1.0f;

        float negativeOutput = HelperFunctions.ZeroOrMore(negativeCase);
        float zeroOutput = HelperFunctions.ZeroOrMore(zeroCase);
        float positiveOutput = HelperFunctions.ZeroOrMore(positiveCase);

        Assert.AreEqual(expectedOutputNegative, negativeOutput);
        Assert.AreEqual(expectedOutputZero, zeroOutput);
        Assert.AreEqual(expectedOutputPositive, positiveOutput);
    }
}
