using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProceduralGenerationTests
{
    // We need to mock a service locator, the weather service depends on it
    GameObject serviceLocator = MockServiceLocator();
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

    public static GameObject MockServiceLocator()
    {
        GameObject go = new GameObject();
        ServiceLocator sl = go.AddComponent<ServiceLocator>();
        GameObject terrainManager = new GameObject();
        TerrainGenerationManager tgm = terrainManager.AddComponent<TerrainGenerationManager>();
        terrainManager.transform.SetParent(go.transform);

        tgm.heightTemperatureSlope = 1f;
        tgm.humidityPhaseShift = 0.5f;

        sl.Awake();
        return go;
        
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
        float expectedMidpoint = positiveTestValue;
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

        float expectedHigh = 0.975f;
        float expectedLow = 0.0f;
        float expectedLowPositive = 0.5f * MathF.Tanh(4 * (1-lowPositiveTestValue) - 2) + 0.5f;
        float expectedHighPositive = 0.5f * MathF.Tanh(4 * (1-positiveTestValue) - 2) + 0.5f;
        float lowGradient = fader.Fade(lowPositiveTestValue + 0.1f) - fader.Fade(lowPositiveTestValue - 0.1f);
        float highGradient = fader.Fade(positiveTestValue + 0.1f) - fader.Fade(positiveTestValue - 0.1f);

        //Negative values means that the test point is within the mask.
        //No diminishing of the signal should happen
        Assert.AreEqual(1.0f, fader.Fade(negativeTestValue));
        Assert.Greater(fader.Fade(zeroTestValue), expectedHigh);

        //Positive values means that the test point is outside the mask
        //Hyperbolic fade attenuates in a changing gradient within the range [0,1]
        Assert.AreEqual(expectedLowPositive, fader.Fade(lowPositiveTestValue));
        Assert.AreEqual(expectedHighPositive, fader.Fade(positiveTestValue));
        Assert.AreEqual(expectedLow, fader.Fade(largePositiveTestvalue));

        //Test for the changing gradient
        Assert.AreNotEqual(lowGradient, highGradient);
    }

    [Test]
    public void TestGaussianFader()
    {
        IFaderStrategy fader = FaderFactory.MakeFader(FaderType.Hyperbolic);
        float negativeTestValue = -1.0f;
        float zeroTestValue = 0.0f;
        float lowPositiveTestValue = 0.2f;
        float positiveTestValue = 0.8f;

        float expectedHigh = 0.975f;

        //Negative values means that the test point is within the mask.
        //No diminishing of the signal should happen
        Assert.AreEqual(1.0f, fader.Fade(negativeTestValue));
        Assert.Greater(fader.Fade(zeroTestValue), expectedHigh);

        //Positive values means that the test point is outside the mask
        //Gaussian fade attenuates in a changing gradient within the range [0,1]
        Assert.Greater(fader.Fade(lowPositiveTestValue), 1-lowPositiveTestValue);
        Assert.Less(fader.Fade(positiveTestValue), 1-positiveTestValue);
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
    public void TestSingleRectangularMask()
    {
        // Arrange masks
        MaskConfig config = new MaskConfig();
        config.center = new Vector2(360, 360);
        config.sizeVariable1 = 40;
        config.sizeVariable2 = 60;
        config.isNegative = false;
        IFaderStrategy linearFade = FaderFactory.MakeFader(FaderType.Linear);
        Mask linearMask = GeneratorFactory
            .MakeMaskGenerator(MaskGeneratorType.Rectangular, linearFade);
        linearMask.ConfigureMask(config);

        // Arrange test cases
        Vector2 withinBounds = new Vector2(config.center.x - config.sizeVariable1 * 0.5f, config.center.y);
        Vector2 outsideWidthBounds = new Vector2(config.center.x + config.sizeVariable1 * 1.5f, config.center.y);
        Vector2 outsideHeightBounds = new Vector2(config.center.x, config.center.y + config.sizeVariable2 * 1.5f);
        Vector2 diagonalDistance = new Vector2(config.center.x + config.sizeVariable1 * 1.5f, 
                                                config.center.y + config.sizeVariable2 * 1.5f);

        Assert.AreEqual(1.0f, linearMask.GeneratePixelValue((int)withinBounds.x, (int)withinBounds.y));
        Assert.Less(linearMask.GeneratePixelValue((int)outsideHeightBounds.x, (int)outsideHeightBounds.y),
            linearMask.GeneratePixelValue((int)withinBounds.x, (int)withinBounds.y));
        Assert.AreEqual(
            linearMask.GeneratePixelValue(
                (int)outsideHeightBounds.x, (int)outsideHeightBounds.y), 
            linearMask.GeneratePixelValue(
                (int)outsideWidthBounds.x, (int)outsideWidthBounds.y));
        
        Assert.Less(linearMask.GeneratePixelValue(
                (int)diagonalDistance.x, (int)diagonalDistance.y),
            linearMask.GeneratePixelValue(
                (int)outsideWidthBounds.x, (int)outsideWidthBounds.y));

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
        float width = 20f;
        float height = 20f;
        Generator generator = new Generator(new Vector2(width, height));
        //We create the perlin generator separately as this is a randomness test and randomness doesn't always behave
        PerlinNoiseGenerator perlinNoiseGenerator = new PerlinNoiseGenerator();
        perlinNoiseGenerator.SetSeed(new Vector2(10f, 10f));
        generator.SetNoiseGenerator(perlinNoiseGenerator);

        // Arrange testing data
        float deltaToleranceThreshold = 0.5f; // This is a generous upper threshold but it ensures no two data points are too far apart
        
        float[,] gridOfTestValues = generator.GenerateNoiseArray();
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

        // Big old dataset (1600 entries)
        float width = 40f;
        float height = 40f;

        int[] bins = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        //We create the perlin generator separately as this is a randomness test and randomness doesn't always behave
        PerlinNoiseGenerator perlinNoiseGenerator = new PerlinNoiseGenerator();
        perlinNoiseGenerator.SetSeed(new Vector2(10f, 10f));
        Generator generator = new Generator(new Vector2(width, height));
        generator.SetNoiseGenerator(perlinNoiseGenerator);

        float[,] testValues = generator.GenerateNoiseArray();
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
        Vector2 testDimensions = new Vector2(40, 40);
        Generator perlinGenerator = new Generator(testDimensions);
        perlinGenerator.SetNoiseGenerator(GeneratorFactory.MakeNoiseGenerator(NoiseGeneratorType.Perlin));
        Generator randomGenerator = new Generator(testDimensions);
        

        float[,] outputPerlin = perlinGenerator.GenerateNoiseArray();
        float[,] outputRandom = randomGenerator.GenerateNoiseArray();

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
    #region Weather Service
    [Test]
    public void TestSlowBoundExpansion()
    {
        // These test parameters will probably shift as we refine the game
        int firstTurn = 1;
        int earlyTurn = 10;
        int midGame = 30;
        int lateGame = 60;

        Assert.Less(WeatherService.CalculateTemperatureBoundExpansion(firstTurn), 2);
        Assert.Less(WeatherService.CalculateTemperatureBoundExpansion(earlyTurn),2);
        Assert.Less(WeatherService.CalculateTemperatureBoundExpansion(midGame), 2);
        Assert.Less(WeatherService.CalculateTemperatureBoundExpansion(lateGame), 2);

        Assert.Greater(WeatherService.CalculateTemperatureBoundExpansion(midGame), 1);
        Assert.Greater(WeatherService.CalculateTemperatureBoundExpansion(lateGame), 1);
    }

    [Test]
    public void TestTemperatureSimulation()
    {
        // Temperature maximums and minimums should increase with every turn.
        // Even turns are maximums, odd turns are minimums.

        float prevBoundExpansion;
        float currentBoundExpansion;
        float currentValue;
        float previousValue;
        float lowHeight = 0.0f;
        float highHeight = 1.0f;
        int maxTest = 60;

        float setBoundExpansion = WeatherService.CalculateTemperatureBoundExpansion(1);
        float lowHeightTemperatureValue = WeatherService.CalculateTemperature(1, 0, setBoundExpansion, lowHeight);
        float highHeightTemperatureValue = WeatherService.CalculateTemperature(1, 0, setBoundExpansion, highHeight);

        for (int i = 2; i < maxTest; i += 2)
        {
            prevBoundExpansion = WeatherService.CalculateTemperatureBoundExpansion(i - 2);
            currentBoundExpansion = WeatherService.CalculateTemperatureBoundExpansion(i);
            previousValue = WeatherService.CalculateTemperature(i - 2, 0, prevBoundExpansion, 0.0f);
            currentValue = WeatherService.CalculateTemperature(i, 0, currentBoundExpansion, 0.0f);
            Assert.Greater(currentValue, previousValue);
        }

        for (int i = 3; i < maxTest; i += 2)
        {
            prevBoundExpansion = WeatherService.CalculateTemperatureBoundExpansion(i - 2);
            currentBoundExpansion = WeatherService.CalculateTemperatureBoundExpansion(i);
            previousValue = WeatherService.CalculateTemperature(i - 2, 0, prevBoundExpansion, 0.0f);
            currentValue = WeatherService.CalculateTemperature(i, 0, currentBoundExpansion, 0.0f);
            Assert.Less(currentValue, previousValue);
        }

        // Higher heights should show less temperature
        Assert.Less(highHeightTemperatureValue, lowHeightTemperatureValue);
    }

    [Test]
    public void TestHumiditySimulation()
    {

        int firstTurn = 1;
        int maxTurn = 60;
        float lowHeight = 0.0f;
        float midHeight = 0.5f;
        float highHeight = 1.0f;

        float earlyHumidityLowHeight = WeatherService.CalculateHumidity(firstTurn, lowHeight);
        float earlyHumidityMidHeight = WeatherService.CalculateHumidity(firstTurn, midHeight);
        float earlyHumidityHighHeight = WeatherService.CalculateHumidity(firstTurn, highHeight);

        //Humidity should decrease as we increase in height
        Assert.Less(earlyHumidityMidHeight, earlyHumidityLowHeight);
        Assert.Less(earlyHumidityHighHeight, earlyHumidityMidHeight);

        // Values should be bound between 0 and 1
        for (int i = 0; i < maxTurn; i++)
        {
            float cycleValue = WeatherService.CalculateHumidity(i, lowHeight);
            Assert.LessOrEqual(cycleValue, 1);
            Assert.GreaterOrEqual(cycleValue, 0);
        }
    }

    [Test]
    public void TestTemperatePrecipitationChance()
    {

        // Setting is early game and height is attenuating temperature and humidity
        int temperateTurn = 2;
        float testHeight = 0.2f;
        float temperatureBounds = WeatherService.CalculateTemperatureBoundExpansion(temperateTurn);
        float temperature = WeatherService.CalculateTemperature(temperateTurn, 0, temperatureBounds, testHeight);
        float humidity = WeatherService.CalculateHumidity(temperateTurn, testHeight);

        float chanceOfPrecipitation = WeatherService.CalculatePrecipitationChance(temperature, humidity);

        // There should be a very, very low chance of precipitation
        Assert.Less(chanceOfPrecipitation, 0.05f);

    }

    [Test]
    public void TestExtremeConditionPrecipitationChance()
    {
        // Setting is late game, corresponding to a turn with high humidity and no attenuating height is present
        int lateTurn = 54;
        float testHeight = 0.0f;
        float temperatureBounds = WeatherService.CalculateTemperatureBoundExpansion(lateTurn);
        float temperature = WeatherService.CalculateTemperature(lateTurn, 0, temperatureBounds, testHeight);
        Debug.Log(temperature);
        float humidity = WeatherService.CalculateHumidity(lateTurn, testHeight);

        float chanceOfPrecipitation = WeatherService.CalculatePrecipitationChance(temperature, humidity);

        // There should be a very, very low chance of precipitation
        Assert.Greater(chanceOfPrecipitation, 0.5f);
    }
    #endregion
}

