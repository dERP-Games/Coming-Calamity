using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class TraitSystemTests
{
    Trait LoadTestTrait(string traitName)
    {
        Trait trait = (Trait)AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjects/Traits/" + traitName + ".asset", typeof(Trait));
        return trait;
    }

    [Test]
    public void TestTraitContainer()
    {
        GameObject go = new GameObject();
        TraitContainer tc = go.AddComponent<TraitContainer>();
        GameObject text_go = new GameObject();
        TextMeshProUGUI textContainer = text_go.AddComponent<TextMeshProUGUI>();
        GameObject img_go = new GameObject();
        Image im = img_go.AddComponent<Image>();

        tc.traitTitleContainer = textContainer;
        tc.traitImage = im;
        Trait mobilityTrait = LoadTestTrait("TestTrait1");

        tc.SetTrait(mobilityTrait);

        Assert.AreEqual(tc.traitTitleContainer.text, mobilityTrait.traitName);
        Assert.AreEqual(tc.trait.statGroup, mobilityTrait.statGroup);

    }
}
