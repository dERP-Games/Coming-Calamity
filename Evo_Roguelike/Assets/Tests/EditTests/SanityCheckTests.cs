using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SanityCheckTests
{

    [Test]
    public void BasicSumCheck()
    {
        int expectedSum = 4;

        Assert.AreEqual(expectedSum, 2 + 2);
    }

    [Test]
    public void BasicStringConcatenationCheck()
    {
        string firstPart = "Hello";
        string secondPart = " World!";
        string expectedString = "Hello World!";

        Assert.AreEqual(expectedString, firstPart + secondPart);
    }
}
