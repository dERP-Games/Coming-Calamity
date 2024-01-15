// Support Classes and Data Types
using System;
using UnityEngine;
using UnityEngine.UI;

public struct NormalizingValues
{
    // Simple struct to return the values needed to normalize
    // the output of a Generator
    public float min;
    public float max;

    public NormalizingValues(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}

// Enums for the factories
public enum NoiseGeneratorType
{
    Default,
    Perlin
}

public enum MaskGeneratorType
{
    Default,
    Radial,
    Elliptical, // To do
    Rectangular
}

public enum FaderType
{
    Default,
    Linear,
    Hyperbolic,
    Gaussian
}

// High level classes
public interface IGeneratorStrategy
{
    // A Generator Strategy the mechanism the Generator
    // class uses to create the value for a "pixel" in the map

    // This function takes location coordinates and outputs the "height" value
    // at that coordinate.
    public float GeneratePixelValue(int x, int y);

    // Some generators can produce values outside of the range 0-1
    // To ensure output remains within the desired 0-1 range, the Generator
    // can use these normalizing to remap output data.
    public NormalizingValues GetNormalizingValues();

}

public interface IFaderStrategy
{
    // Faders fade out a mask in proportion to some distance
    // to its bounds.
    public float Fade(float distance);
    public void Configure(MaskConfig config);
}

public static class GeneratorFactory
{
    // The generator factory is the class through which GeneratorStrategies
    // are produced.
    public static IGeneratorStrategy MakeNoiseGenerator(NoiseGeneratorType type)
    {
        // Noise generators can be extended if we find that perlin is not good enough
        switch (type)
        {
            case NoiseGeneratorType.Perlin:
                return new PerlinNoiseGenerator();
            default:
                return new RandomNoiseGenerator();
        }
    }

    public static Mask MakeMaskGenerator(MaskGeneratorType type, IFaderStrategy fader)
    {
        // Masks act in combination with the noise generators to give shape to the terrain.
        switch (type)
        {
            case MaskGeneratorType.Radial:
                return new RadialMask(fader);
            case MaskGeneratorType.Rectangular:
                return new RectangularMask(fader);
            default:
                return new NoMask(fader);
        }
    }
}

public static class FaderFactory
{
    public static IFaderStrategy MakeFader(MaskConfig config)
    {
        IFaderStrategy fader;
        switch (config.faderType)
        {
            case FaderType.Hyperbolic:
                fader = new HyperbolicFader();
                break;
            case FaderType.Linear:
                fader = new LinearFader();
                break;
            case FaderType.Gaussian:
                fader = new GaussianFader();
                break;
            default:
                fader = new NoFade();
                break;
        }
        fader.Configure(config);
        return fader;
    }

    public static IFaderStrategy MakeFader(FaderType type)
    {
        switch (type)
        {
            case FaderType.Hyperbolic:
                return new HyperbolicFader();
            case FaderType.Linear:
                return new LinearFader();
            case FaderType.Gaussian:
                return new GaussianFader();
            default:
                return new NoFade();
        }
    }
}

// Faders
public class NoFade : IFaderStrategy
{
    // Does nothing. Yay!
    public float Fade(float distance)
    {
        if (distance <= 0)
        {
            return 1.0f;
        }
        return 0.0f;
    }

    public void Configure(MaskConfig config){}
}

public class LinearFader : IFaderStrategy
{
    // Linearly fade away from 1
    private float _gradient;
    public LinearFader()
    {
        _gradient = 1.0f;
    }
    public float Fade(float distance)
    {
        return Mathf.Clamp(1.0f - (distance * _gradient), 0.0f, 1.0f);
    }
    public void Configure(MaskConfig config)
    {
        _gradient = config.faderGradient;
    }
}

public class HyperbolicFader : IFaderStrategy
{
    // This class works very weird when given gradients under 0.8f - be aware
    float _gradient;


    // Ease out of 1 and ease into 0, but steep gradient through the middle
    public HyperbolicFader()
    {
        _gradient = 1.0f;
    }
    public float Fade(float distance)
    {
        if (distance < 0)
        {
            return 1.0f;
        }
        float inverted_distance = 1 - distance;
        // The output function of this fader can be seen here https://www.desmos.com/calculator/hi3mksaava
        return Mathf.Clamp(0.5f * MathF.Tanh(4*_gradient * inverted_distance - 2*_gradient) + 0.5f, 0f, 1f); 
    }

    public void Configure(MaskConfig config)
    {
        _gradient = config.faderGradient;
    }
}

public class GaussianFader : IFaderStrategy
{
    // The gradient value in this fader is inverted - higher values produce flatter curves. Gaussian is weird, what can I say?
    // Also the gradient is VERY sensitive. Recommended ranges are between 0.3 and 0.6;
    float _gradient;
    public GaussianFader()
    {
        _gradient = 0.4f;
    }
    public float Fade(float distance)
    {
        if (distance < 0)
        {
            return 1.0f;
        }
        float inverted_distance = 1 - distance;
        float coefficient = (2.5f * _gradient) / (_gradient * Mathf.Sqrt(2 * Mathf.PI));
        float power = -0.5f * Mathf.Pow((inverted_distance - 1) / _gradient, 2);
        // The output function of this fader can be seen here https://www.desmos.com/calculator/snybdgcxr6
        // and is constrained to the domain of [0,1]
        return coefficient * Mathf.Exp(power);
    }

    public void Configure(MaskConfig config)
    {
        _gradient = config.faderGradient;
    }
}

// Noise Generators
public class RandomNoiseGenerator : IGeneratorStrategy
{
    // Pure random noise generator. Will rarely be used but is a useful default scenario.
    public float GeneratePixelValue(int x, int y)
    {
        return UnityEngine.Random.Range(0.0f, 1.0f);
    }

    public NormalizingValues GetNormalizingValues()
    {
        return new NormalizingValues(0.0f, 1.0f);
    }
}

public class PerlinNoiseGenerator : IGeneratorStrategy
{
    // Extension wrapping for the Unity Perlin noise generator
    // Adds FBM to the output.
    // Mind you, Unity PerlinNoise is not random.
    // Textually "The same coordinates will always return the same sample value" (https://docs.unity3d.com/ScriptReference/Mathf.PerlinNoise.html)
    // So we create offsets using a random seed determined at construction

    private Vector2 _seed;
    private float _minValue = 10;
    private float _maxValue = -10;
    public PerlinNoiseGenerator()
    {
        _seed = new Vector2(UnityEngine.Random.Range(0, 9999f), UnityEngine.Random.Range(0, 9999f));
    }

    // Debugging method to ensure consistent output
    public void SetSeed(Vector2 value)
    {
        _seed = value;
    }

    public float GeneratePixelValue(int x, int y)
    {
        //float noise = FractalBrownianMotion(x, y);
        float noise = FractalBrownianMotion(x, y);
        UpdateNormalizingValues(noise);
        return noise;
    }

    public NormalizingValues GetNormalizingValues()
    {
        return new NormalizingValues(_minValue, _maxValue);
    }

    private float FractalBrownianMotion(float x, float y)
    {
        float result = 0.0f;
        float frequency = PCGConfig.starting_frequency;
        float newX;
        float newY;
        float amplitude = PCGConfig.amplitude_factor;

        for (int i = 0; i < PCGConfig.octaves; i++)
        {
            newX = _seed.x + x / PCGConfig.scale * frequency;
            newY = _seed.y + y / PCGConfig.scale * frequency;
            result += amplitude * NoiseAtCoordinate(newX, newY);
            frequency *= PCGConfig.lacunarity;
            amplitude *= PCGConfig.amplitude_factor;
        }
        return result;
        
    }

    private float NoiseAtCoordinate(float x, float y)
    {

        
        float sample = Mathf.PerlinNoise(_seed.x + x, _seed.y + y) * 2 - 1;
        return sample;
    }

    private void UpdateNormalizingValues(float noise)
    {
        if (noise < _minValue)
        {
            _minValue = noise;
        } else if (noise > _maxValue)
        {
            _maxValue = noise;
        }
    }
}

// Masks
public abstract class Mask : IGeneratorStrategy
{
    // A mask creates an area that allows noise values to pass through.
    // Values that lie outside its region fade out according to the fade strategy of the mask
    protected IFaderStrategy _fader;
    protected bool isNegative;
    public Mask(IFaderStrategy fader)
    {
        _fader = fader;
    }

    public NormalizingValues GetNormalizingValues()
    {
        return new NormalizingValues(0.0f, 1.0f);
    }

    public abstract float GeneratePixelValue(int x, int y);

    public virtual void ConfigureMask(MaskConfig config) { }
}

public class NoMask : Mask
{
    // Does nothing. Yay!
    // That is, this mask does not affect the output - it returns the input as is.
    public NoMask(IFaderStrategy fader) : base(fader){}
    public override float GeneratePixelValue(int x, int y)
    {
        return 1.0f;
    }
}

public class RadialMask : Mask
{
    float _radius;
    Vector2 _center;
    // A circular mask
    public RadialMask(IFaderStrategy fader) : base(fader){}

    public override float GeneratePixelValue(int x, int y)
    {
        // Distance is calculated as a factor of the radius
        Vector2 coordinates = new Vector2(x, y);
        float distance = Vector2.Distance(coordinates, _center);
        float radius_proportional_distance = (distance / _radius) - 1.0f;
        float output = _fader.Fade(radius_proportional_distance);
        
        //Evaluate if this is subtractive or additive
        output = isNegative ? -1 * output : output;
        return output;
    }

    public override void ConfigureMask(MaskConfig config)
    {
        _radius = config.sizeVariable1;
        _center = config.center;
        isNegative = config.isNegative;
    }
}

public class RectangularMask : Mask
{
    float _height;
    float _width;
    Vector2 _center;
    public RectangularMask(IFaderStrategy fader ) : base(fader) { }

    public override float GeneratePixelValue(int x, int y)
    {
        Vector2 coordinate = new Vector2(x, y);
        float xDistance = Mathf.Abs(coordinate.x - _center.x);
        xDistance = (xDistance / _width) - 1.0f;
        float xFade = _fader.Fade(xDistance);
        float yDistance = Mathf.Abs(coordinate.y - _center.y);
        yDistance = (yDistance / _height) - 1.0f;
        float yFade = _fader.Fade(yDistance);

        float output = xFade * yFade;

        output = isNegative ? -1 * output : output;
        return output;
    }

    public override void ConfigureMask(MaskConfig config)
    {
        _width = config.sizeVariable1;
        _height = config.sizeVariable2;
        _center = config.center;
        isNegative = config.isNegative;
    }
}

public class Generator
{
    public IGeneratorStrategy noiseGenerationStrategy;
    Mask[] masks;
    public float[,] noiseMap = new float[0,0];
    private Vector2 dimensions;

    public Generator(Vector2 dimensions, NoiseGeneratorType noiseType, MaskSetup maskSetup)
    {
        this.dimensions = dimensions;
        noiseGenerationStrategy = GeneratorFactory.MakeNoiseGenerator(noiseType);
        masks = new Mask[maskSetup.maskConfig.Length];
        for (int i = 0; i < maskSetup.maskConfig.Length; i++)
        {
            MaskConfig config = maskSetup.maskConfig[i];
            IFaderStrategy fader = FaderFactory.MakeFader(config);
            masks[i] = GeneratorFactory.MakeMaskGenerator(config.maskType, fader);
            masks[i].ConfigureMask(config);
        }
        
    }

    // Additional constructor using all default types. Likely won't be used in production but useful for testing
    public Generator(Vector2 dimensions)
    {
        this.dimensions = dimensions;
        noiseGenerationStrategy = GeneratorFactory.MakeNoiseGenerator(NoiseGeneratorType.Default);
        masks = new Mask[1];
        IFaderStrategy fader = FaderFactory.MakeFader(FaderType.Default);
        masks[0] = GeneratorFactory.MakeMaskGenerator(MaskGeneratorType.Default, fader);
    }

    // Additional constructor to allow for custom setting of noise generators (mostly for debugging purposes)
    public Generator(Vector2 dimensions, MaskSetup maskSetup)
    {
        this.dimensions = dimensions;
        masks = new Mask[maskSetup.maskConfig.Length];
        for (int i = 0; i < maskSetup.maskConfig.Length; i++)
        {
            MaskConfig config = maskSetup.maskConfig[i];
            IFaderStrategy fader = FaderFactory.MakeFader(config.faderType);
            masks[i] = GeneratorFactory.MakeMaskGenerator(config.maskType, fader);
            masks[i].ConfigureMask(config);
        }
    }

    public void SetNoiseGenerator(IGeneratorStrategy gs)
    {
        noiseGenerationStrategy = gs;
    }

    public void VoidCache()
    {
        noiseMap = new float[0, 0];
    }

    public float[,] GenerateNoiseArray()
    {
        // This function and the following create the noise array
        // This version takes a Vector2 to describe the canvas size

        // Use a cache first if one exists
        if (noiseMap.Length > 0)
        {
            return noiseMap;
        }
        
        int width = (int)dimensions.x;
        int height = (int)dimensions.y;
        float[,] noiseOutput = new float[width,height];
        PopulateNoiseValues(width, height, noiseOutput);
        return noiseOutput;
    }

    public float[,] GenerateNoiseArray(int width, int height)
    {
        // This version takes an int for width and an int for height.
        // Both versions essentially just call the underlying PopulateNoiseValues function

        // Use a cache first if one exists
        if (noiseMap.Length > 0)
        {
            return noiseMap;
        }

        float[,] noiseOutput = new float[width, height];
        PopulateNoiseValues(width, height, noiseOutput);
        return noiseOutput;
    }

    private void PopulateNoiseValues(int width, int height, float[,] container)
    {
        // This function populates a grid of noise values
        // It takes as inputs the dimensions of the grid and the array that represents it
        // It outputs nothing but modifies the input array directly

        // Generate the noise values
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                container[j, i] = noiseGenerationStrategy.GeneratePixelValue(j, i);
            }
        }

        //Normalize and mask them
        NormalizingValues norm = noiseGenerationStrategy.GetNormalizingValues();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                container[j, i] = Mathf.InverseLerp(norm.min, norm.max, container[j, i]);
                container[j, i] *= MaskEvaluation(j, i);
            }
        }

        //Cache the created noiseMap
        noiseMap = container;
    }

    private float MaskEvaluation(int x, int y)
    {
        // Consolidate the output of all masks at this location, total output should be between [-1,1]
        float summedMaskOutput = 0f;
        for (int m = 0; m < masks.Length; m++)
        {
            summedMaskOutput += masks[m].GeneratePixelValue(x, y);
        }
        summedMaskOutput = Mathf.Clamp(summedMaskOutput, -1, 1);
        return summedMaskOutput;
    }
}