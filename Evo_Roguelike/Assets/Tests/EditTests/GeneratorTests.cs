using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProceduralGenerationTests
{
    #region Fixtures
    public MaskConfig[] SingleRadialMaskConfig()
    {
        MaskConfig[] mask = new MaskConfig[]
        {
            new MaskConfig()
        };
        mask[0].center = new Vector2(360, 360);
        mask[0].sizeVariable1 = 200.0f;
        mask[0].isNegative = false;
        return mask;
    }

    public MaskConfig[] MultiRadialMasksAllPositive()
    {
        MaskConfig[] mask = new MaskConfig[]
        {
            new MaskConfig(),
            new MaskConfig()
        };
        //Mask 1
        mask[0].center = new Vector2(180, 180);
        mask[0].sizeVariable1 = 75f;
        mask[0].isNegative = false;

        // Mask 2
        mask[1].center = new Vector2(360, 360);
        mask[1].sizeVariable1 = 70f;
        mask[1].isNegative = false;
        return mask;
    }

    public MaskConfig[] MultiRadialMasksMixedSign()
    {
        MaskConfig[] mask = new MaskConfig[]
        {
            new MaskConfig(),
            new MaskConfig()
        };
        //Mask 1
        mask[0].center = new Vector2(180, 180);
        mask[0].sizeVariable1 = 75f;
        mask[0].isNegative = true;

        // Mask 2
        mask[1].center = new Vector2(360, 360);
        mask[1].sizeVariable1 = 70f;
        mask[1].isNegative = false;
        return mask;
    }
    #endregion
    #region Fader Tests
    [Test]
    public void TestFadeFactory()
    {
        // All factory methods execute without throwing an error
        Assert.DoesNotThrow(() => FaderFactory.MakeFader(FaderType.Linear));
        Assert.DoesNotThrow(() => FaderFactory.MakeFader(FaderType.Hyperbolic));
        Assert.DoesNotThrow(() => FaderFactory.MakeFader(FaderType.Default));
    }

    [Test]
    public void TestNoFade()
    {
        IFaderStrategy fader = FaderFactory.MakeFader(FaderType.Default);
        float expectedHigh = 1.0f;
        float expectedLow = 0.0f;
        
        float negativeTestValue = -1.0f;
        float zeroTestValue = 0.0f;
        float positiveTestValue = 0.5f;
        float largePositiveTestvalue = 5.5f;

        //Negative values means that the test point is within the mask.
        //No diminishing of the signal should happen
        Assert.AreEqual(expectedHigh, fader.Fade(negativeTestValue));
        Assert.AreEqual(expectedHigh, fader.Fade(zeroTestValue));

        //Positive values means that the test point is outside the mask
        //NoFade sets values outside the mask to 0
        Assert.AreEqual(expectedLow, fader.Fade(positiveTestValue));
        Assert.AreEqual(expectedLow, fader.Fade(largePositiveTestvalue));
    }

    [Test]
    public void TestLinearFade()
    {
        IFaderStrategy fader = FaderFactory.MakeFader(FaderType.Linear);
        float negativeTestValue = -1.0f;
        float zeroTestValue = 0.0f;
        float positiveTestValue = 0.5f;
        float largePositiveTestvalue = 5.5f;

        float expectedHigh = 1.0f;
        float expectedLow = 0.0f;
        float expectedMidpoint = positiveTestValue * PCGConfig.fadeGradient;
        float lowGradient = fader.Fade(0.3f) - fader.Fade(0.2f);
        float highGradient = fader.Fade(0.8f) - fader.Fade(0.7f);

        //Negative values means that the test point is within the mask.
        //No diminishing of the signal should happen
        Assert.AreEqual(expectedHigh, fader.Fade(negativeTestValue));
        Assert.AreEqual(expectedHigh, fader.Fade(zeroTestValue));

        //Positive values means that the test point is outside the mask
        //Linear fade attenuates the signal outside the mask linearly
        Assert.AreEqual(expectedMidpoint, fader.Fade(positiveTestValue));
        Assert.AreEqual(expectedLow, fader.Fade(largePositiveTestvalue));

        //Test that gradient is the same in all locations of the fader
        Assert.AreEqual(lowGradient, highGradient);
    }

    [Test]
    public void TestHyperbolicFade()
    {
        IFaderStrategy fader = FaderFactory.MakeFader(FaderType.Hyperbolic);
        float negativeTestValue = -1.0f;
        float zeroTestValue = 0.0f;
        float lowPositiveTestValue = 0.2f;
        float positiveTestValue = 0.6f;
        float largePositiveTestvalue = 5.5f;

        float expectedHigh = 1.0f;
        float expectedLow = 0.0f;
        float expectedLowPositive = 0.5f * MathF.Tanh(4 * (1-lowPositiveTestValue) - 2) + 0.5f;
        float expectedHighPositive = 0.5f * MathF.Tanh(4 * (1-positiveTestValue) - 2) + 0.5f;
        float lowGradient = fader.Fade(lowPositiveTestValue + 0.1f) - fader.Fade(lowPositiveTestValue - 0.1f);
        float highGradient = fader.Fade(positiveTestValue + 0.1f) - fader.Fade(positiveTestValue - 0.1f);

        //Negative values means that the test point is within the mask.
        //No diminishing of the signal should happen
        Assert.AreEqual(expectedHigh, fader.Fade(negativeTestValue));
        Assert.AreEqual(expectedHigh, fader.Fade(zeroTestValue));

        //Positive values means that the test point is outside the mask
        //Hyperbolic fade attenuates in a changing gradient within the range [0,1]
        Assert.AreEqual(expectedLowPositive, fader.Fade(lowPositiveTestValue));
        Assert.AreEqual(expectedHighPositive, fader.Fade(positiveTestValue));
        Assert.AreEqual(expectedLow, fader.Fade(largePositiveTestvalue));

        //Test for the changing gradient
        Assert.AreNotEqual(lowGradient, highGradient);
    }
    #endregion
    #region Mask Tests
    [Test]
    public void TestMaskFactory()
    {
        IFaderStrategy noFade = FaderFactory.MakeFader(FaderType.Default);
        Assert.DoesNotThrow(() => GeneratorFactory
                .MakeMaskGenerator(MaskGeneratorType.Default, noFade));
        Assert.DoesNotThrow(() => GeneratorFactory
                .MakeMaskGenerator(MaskGeneratorType.Elliptical, noFade));
        Assert.DoesNotThrow(() => GeneratorFactory
                .MakeMaskGenerator(MaskGeneratorType.Radial, noFade));
        Assert.DoesNotThrow(() => GeneratorFactory
                .MakeMaskGenerator(MaskGeneratorType.Rectangular, noFade));
    }

    [Test]
    public void TestNoMask()
    {
        // Arrange masks
        IFaderStrategy noFade = FaderFactory.MakeFader(FaderType.Default);
        IFaderStrategy linearFade = FaderFactory.MakeFader(FaderType.Linear);
        IFaderStrategy hbFader = FaderFactory.MakeFader(FaderType.Hyperbolic);
        Mask pureMask = GeneratorFactory
            .MakeMaskGenerator(MaskGeneratorType.Default, noFade);
        Mask linearMask = GeneratorFactory
            .MakeMaskGenerator(MaskGeneratorType.Default, linearFade);
        Mask hbMask = GeneratorFactory
            .MakeMaskGenerator(MaskGeneratorType.Default, hbFader);

        //Arrange test cases
        float expectedValue = 1.0f;

        //Test Locations
        Vector2 centralPosition = new Vector2(360.0f, 360.0f);
        Vector2 fringePosition1 = new Vector2(0.0f, 0.0f);
        Vector2 fringePosition2 = new Vector2(720.0f, 720.0f);

        // Act - Calculate mask outputs
        float pureCentralOutput = pureMask.GeneratePixelValue(
            (int) centralPosition.x, (int) centralPosition.y);
        float pureFringeOutput1 = pureMask.GeneratePixelValue(
            (int) fringePosition1.x, (int) fringePosition2.y);
        float pureFringeOutput2 = pureMask.GeneratePixelValue(
            (int)fringePosition2.x, (int)fringePosition2.y);

        float linearCentralOutput = linearMask.GeneratePixelValue(
            (int)centralPosition.x, (int)centralPosition.y);
        float linearFringeOutput1 = linearMask.GeneratePixelValue(
            (int)fringePosition1.x, (int)fringePosition2.y);
        float linearFringeOutput2 = linearMask.GeneratePixelValue(
            (int)fringePosition2.x, (int)fringePosition2.y);

        float hbCentralOutput = hbMask.GeneratePixelValue(
            (int)centralPosition.x, (int)centralPosition.y);
        float hbFringeOutput1 = hbMask.GeneratePixelValue(
            (int)fringePosition1.x, (int)fringePosition2.y);
        float hbFringeOutput2 = hbMask.GeneratePixelValue(
            (int)fringePosition2.x, (int)fringePosition2.y);

        // Assert
        Assert.AreEqual(expectedValue, pureCentralOutput);
        Assert.AreEqual(expectedValue, pureFringeOutput1);
        Assert.AreEqual(expectedValue, pureFringeOutput2);

        Assert.AreEqual(expectedValue, linearCentralOutput);
        Assert.AreEqual(expectedValue, linearFringeOutput1);
        Assert.AreEqual(expectedValue, linearFringeOutput2);

        Assert.AreEqual(expectedValue, hbCentralOutput);
        Assert.AreEqual(expectedValue, hbFringeOutput1);
        Assert.AreEqual(expectedValue, hbFringeOutput2);

    }

    [Test]
    public void TestSingleRadialMask()
    {
        MaskConfig[] maskConfig = SingleRadialMaskConfig();
        // Arrange masks
        IFaderStrategy noFade = FaderFactory.MakeFader(FaderType.Default);
        IFaderStrategy linearFade = FaderFactory.MakeFader(FaderType.Linear);
        IFaderStrategy hbFader = FaderFactory.MakeFader(FaderType.Hyperbolic);
        Mask pureMask = GeneratorFactory
            .MakeMaskGenerator(MaskGeneratorType.Radial, noFade);
        pureMask.ConfigureMask(maskConfig[0]);
        Mask linearMask = GeneratorFactory
            .MakeMaskGenerator(MaskGeneratorType.Radial, linearFade);
        linearMask.ConfigureMask(maskConfig[0]);
        Mask hbMask = GeneratorFactory
            .MakeMaskGenerator(MaskGeneratorType.Radial, hbFader);
        hbMask.ConfigureMask(maskConfig[0]);

        //Arrange test cases

        //Half a radius away
        Vector2 withinMaskPosition = new Vector2(
            maskConfig[0].center.x - maskConfig[0].sizeVariable1 * 0.5f,
            maskConfig[0].center.y);

        //1.3 radii away
        Vector2 outsideMaskPosition = new Vector2(
            maskConfig[0].center.x - maskConfig[0].sizeVariable1 * 1.3f,
            maskConfig[0].center.y);

        // Arrange expected values
        float expectedOutputWithin = 1.0f;
        float expectedPureMaskOutputOutside = 0.0f;
        float expectedLinearMaskOutputOutside = 0.7f;
        float expectedHyperbolicOutputOutside = 0.5f * MathF.Tanh(4 * 0.7f - 2) + 0.5f;
        float expectedHyperbolicThreshold = 0.8f;

        // Act
        float pureMaskOutputWithin = pureMask.GeneratePixelValue(
            (int)withinMaskPosition.x, 
            (int)withinMaskPosition.y
        );
        float linearMaskOutputWithin = linearMask.GeneratePixelValue(
            (int)withinMaskPosition.x,
            (int)withinMaskPosition.y
        );
        float hbMaskOutputWithin = hbMask.GeneratePixelValue(
            (int)withinMaskPosition.x,
            (int)withinMaskPosition.y
        );

        float pureMaskOutputOutside = pureMask.GeneratePixelValue(
            (int)outsideMaskPosition.x,
            (int)outsideMaskPosition.y
        );
        float linearMaskOutputOutside = linearMask.GeneratePixelValue(
            (int)outsideMaskPosition.x,
            (int)outsideMaskPosition.y
        );
        float hbMaskOutputOutside = hbMask.GeneratePixelValue(
            (int)outsideMaskPosition.x,
            (int)outsideMaskPosition.y
        );

        // Assert
        Assert.AreEqual(expectedOutputWithin, pureMaskOutputWithin);
        Assert.AreEqual(expectedOutputWithin, linearMaskOutputWithin);
        Assert.AreEqual(expectedOutputWithin, hbMaskOutputWithin);

        Assert.AreEqual(expectedPureMaskOutputOutside, pureMaskOutputOutside);
        Assert.AreEqual(expectedLinearMaskOutputOutside, linearMaskOutputOutside);
        Assert.AreEqual(expectedHyperbolicOutputOutside, hbMaskOutputOutside);
        Assert.Greater(hbMaskOutputOutside, expectedHyperbolicThreshold);
    }

    [Test]
    public void TestOverlappingPositiveMasks()
    {
        MaskConfig[] configs = MultiRadialMasksAllPositive();
        
        // We use a linear fader. Other fading strategies and masks have been tested before.
        IFaderStrategy fader = FaderFactory.MakeFader(FaderType.Linear);
        Mask mask1 = GeneratorFactory.MakeMaskGenerator(MaskGeneratorType.Radial, fader);
        mask1.ConfigureMask(configs[0]);
        Mask mask2 = GeneratorFactory.MakeMaskGenerator(MaskGeneratorType.Radial, fader);
        mask2.ConfigureMask(configs[1]);

        // Expected values
        float expectedWithinOutput = 1.0f;
        float expectedOutsideOutput = 0.0f;


        // Test positions: Inside both masks, outside both masks and within fading range of both masks
        Vector2 withinMask1 = new Vector2(configs[0].center.x - configs[0].sizeVariable1 * 0.5f, configs[0].center.y);
        Vector2 withinMask2 = new Vector2(configs[1].center.x - configs[1].sizeVariable1 * 0.5f, configs[1].center.y);
        Vector2 outsideMasks = new Vector2(720f, 720f);
        Vector2 intersectionPoint = new Vector2(
            0.5f * (configs[0].center.x + configs[1].center.x),
            0.5f * (configs[0].center.y + configs[1].center.y)
            );

        float combinedMaskOutputWithinMask1 = mask1.GeneratePixelValue((int) withinMask1.x, (int) withinMask1.y) + 
            mask2.GeneratePixelValue((int) withinMask1.x, (int) withinMask1.y);

        float combinedMaskOutputWithinMask2 = mask2.GeneratePixelValue((int) withinMask2.x, (int) withinMask2.y) +
            mask1.GeneratePixelValue((int) withinMask2.x, (int) withinMask2.y);
        float combinedMaskOutputOutsideMasks = mask2.GeneratePixelValue((int)outsideMasks.x, (int)outsideMasks.y) +
            mask1.GeneratePixelValue((int)outsideMasks.x, (int)outsideMasks.y);

        float mask1OutputIntersectionPoint = mask1.GeneratePixelValue((int) intersectionPoint.x, (int) intersectionPoint.y);
        float mask2OutputIntersectionPoint = mask2.GeneratePixelValue((int) intersectionPoint.x, (int) intersectionPoint.y);
        float combinedOutputIntersectionPoint = mask1OutputIntersectionPoint + mask2OutputIntersectionPoint;

        //Assert
        Assert.AreEqual(expectedWithinOutput, combinedMaskOutputWithinMask1);
        Assert.AreEqual(expectedWithinOutput, combinedMaskOutputWithinMask2);
        Assert.AreEqual(expectedOutsideOutput, combinedMaskOutputOutsideMasks);
        
        Assert.Less(mask1OutputIntersectionPoint, 1f);
        Assert.Less(mask2OutputIntersectionPoint, 1f);
        Assert.Greater(combinedOutputIntersectionPoint, mask1OutputIntersectionPoint);
        Assert.Greater(combinedOutputIntersectionPoint, mask2OutputIntersectionPoint);


    }

    [Test]
    public void TestOverlappingNegativeMasks()
    {
        MaskConfig[] configs = MultiRadialMasksMixedSign();

        // We use a linear fader. Other fading strategies and masks have been tested before.
        IFaderStrategy fader = FaderFactory.MakeFader(FaderType.Linear);
        Mask mask1 = GeneratorFactory.MakeMaskGenerator(MaskGeneratorType.Radial, fader);
        mask1.ConfigureMask(configs[0]);
        Mask mask2 = GeneratorFactory.MakeMaskGenerator(MaskGeneratorType.Radial, fader);
        mask2.ConfigureMask(configs[1]);

        // Expected values
        float expectedWithinPositiveOutput = 1.0f;
        float expectedWithinNegativeOutput = -1.0f;
        float expectedOutsideOutput = 0.0f;


        // Test positions: Inside both masks, outside both masks and within fading range of both masks
        Vector2 withinMask1 = new Vector2(configs[0].center.x - configs[0].sizeVariable1 * 0.5f, configs[0].center.y);
        Vector2 withinMask2 = new Vector2(configs[1].center.x - configs[1].sizeVariable1 * 0.5f, configs[1].center.y);
        Vector2 outsideMasks = new Vector2(720f, 720f);
        Vector2 intersectionPoint = new Vector2(
            0.5f * (configs[0].center.x + configs[1].center.x),
            0.5f * (configs[0].center.y + configs[1].center.y)
            );

        float combinedMaskOutputWithinMask1 = mask1.GeneratePixelValue((int)withinMask1.x, (int)withinMask1.y) +
            mask2.GeneratePixelValue((int)withinMask1.x, (int)withinMask1.y);

        float combinedMaskOutputWithinMask2 = mask2.GeneratePixelValue((int)withinMask2.x, (int)withinMask2.y) +
            mask1.GeneratePixelValue((int)withinMask2.x, (int)withinMask2.y);
        float combinedMaskOutputOutsideMasks = mask2.GeneratePixelValue((int)outsideMasks.x, (int)outsideMasks.y) +
            mask1.GeneratePixelValue((int)outsideMasks.x, (int)outsideMasks.y);

        float mask1OutputIntersectionPoint = mask1.GeneratePixelValue((int)intersectionPoint.x, (int)intersectionPoint.y);
        float mask2OutputIntersectionPoint = mask2.GeneratePixelValue((int)intersectionPoint.x, (int)intersectionPoint.y);
        float combinedOutputIntersectionPoint = mask1OutputIntersectionPoint + mask2OutputIntersectionPoint;

        //Assert
        Assert.AreEqual(expectedWithinNegativeOutput, combinedMaskOutputWithinMask1);
        Assert.AreEqual(expectedWithinPositiveOutput, combinedMaskOutputWithinMask2);
        Assert.AreEqual(expectedOutsideOutput, combinedMaskOutputOutsideMasks);

        Assert.Less(mask1OutputIntersectionPoint, 0f);
        Assert.Less(mask2OutputIntersectionPoint, 1f);
        Assert.Greater(combinedOutputIntersectionPoint, mask1OutputIntersectionPoint);
        Assert.Less(combinedOutputIntersectionPoint, mask2OutputIntersectionPoint);


    }
    #endregion
    #region Generator Tests
    [Test]
    public void TestNoiseGeneratorFactory()
    {
        Assert.DoesNotThrow(() => GeneratorFactory
                .MakeNoiseGenerator(NoiseGeneratorType.Perlin));
        Assert.DoesNotThrow(() => GeneratorFactory
                .MakeNoiseGenerator(NoiseGeneratorType.Default));
    }

    [Test]
    public void TestPerlinNoiseSmoothness()
    {
        Generator generator = new Generator();
        //We create the perlin generator separately as this is a randomness test and randomness doesn't always behave
        PerlinNoiseGenerator perlinNoiseGenerator = new PerlinNoiseGenerator();
        perlinNoiseGenerator.SetSeed(new Vector2(10f, 10f));
        generator.SetNoiseGenerator(perlinNoiseGenerator);

        // Arrange testing data
        float deltaToleranceThreshold = 0.5f; // This is a generous upper threshold but it ensures no two data points are too far apart
        float width = 20f;
        float height = 20f;
        float[,] gridOfTestValues = generator.GenerateNoiseArray(new Vector2(width, height));
        float rightNeighborDelta;
        float downNeighborDelta;

        //Test every neighbor downwards and rightwards that the difference in noise values doesn't exceed the deltaToleranceThreshold
        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < width - 1; j++)
            {
                rightNeighborDelta = Mathf.Abs(gridOfTestValues[i,j] - gridOfTestValues[i,j+1]);
                downNeighborDelta = Mathf.Abs(gridOfTestValues[i,j] - gridOfTestValues[i+1,j]);
                Assert.Less(rightNeighborDelta, deltaToleranceThreshold);
                Assert.Less(downNeighborDelta, deltaToleranceThreshold);
            }
        }
    }

    [Test]
    public void TestValueDistribution()
    {
        // Fractal noise is not uniformly distributed, it's closer to Gaussian noise.
        // This test validates that noise is not uniformly random.

        int[] bins = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        //We create the perlin generator separately as this is a randomness test and randomness doesn't always behave
        PerlinNoiseGenerator perlinNoiseGenerator = new PerlinNoiseGenerator();
        perlinNoiseGenerator.SetSeed(new Vector2(10f, 10f));
        Generator generator = new Generator();
        generator.SetNoiseGenerator(perlinNoiseGenerator);

        // Big old dataset (1600 entries)
        float width = 40f;
        float height = 40f;

        float[,] testValues = generator.GenerateNoiseArray(new Vector2(width, height));
        int binValue;

        //Run a frequency counter
        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < width - 1; j++)
            {
                //Get the first decimal value (i.e. most significant noise value)
                binValue = (int) (testValues[i,j] * 10);
                bins[binValue]++;
            }
        }

        // These two values should be approximately similar in frequency
        float percentageValuesUnderThree = (bins[0] + bins[1] + bins[2]) / (width * height);
        float percentageValuesOverSeven = (bins[8] + bins[9] + bins[10]) / (width * height);
        float tailDeltaThreshold = 0.3f;
        float tailDelta = Mathf.Abs(percentageValuesOverSeven - percentageValuesUnderThree);

        // These values should be more frequently represented than the previous ones
        float percentageCentralValues = (bins[4] + bins[5] + bins[6]) / (width * height);

        // Assert
        Assert.Greater(percentageCentralValues, percentageValuesUnderThree);
        Assert.Greater(percentageCentralValues, percentageValuesOverSeven);
        Assert.Less(tailDelta, tailDeltaThreshold);
    }

    [Test]
    public void TestNoiseValueConstraints()
    {
        Generator perlinGenerator = new Generator();
        perlinGenerator.SetNoiseGenerator(GeneratorFactory.MakeNoiseGenerator(NoiseGeneratorType.Perlin));
        Generator randomGenerator = new Generator();
        Vector2 testDimensions = new Vector2(40, 40);

        float[,] outputPerlin = perlinGenerator.GenerateNoiseArray(testDimensions);
        float[,] outputRandom = randomGenerator.GenerateNoiseArray(testDimensions);

        for (int i = 0; i < testDimensions.x; i++)
        {
            for (int j = 0; j < testDimensions.y; j++)
            {
                Assert.LessOrEqual(outputPerlin[i, j], 1.0f);
                Assert.GreaterOrEqual(outputPerlin[i, j], 0.0f);

                Assert.LessOrEqual(outputRandom[i, j], 1.0f);
                Assert.GreaterOrEqual(outputRandom[i, j], 0.0f);
            }
        }
    }
    #endregion
}

