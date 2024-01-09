// Support Classes and Data Types
using System;
using UnityEngine;

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
    Rectangular // To do
}

public enum FaderType
{
    Default,
    Linear,
    Hyperbolic
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

    public static IGeneratorStrategy MakeMaskGenerator(MaskGeneratorType type, IFaderStrategy fader)
    {
        // Masks act in combination with the noise generators to give shape to the terrain.
        switch (type)
        {
            case MaskGeneratorType.Radial:
                return new RadialMask(fader);
            default:
                return new NoMask(fader);
        }
    }
}

public static class FaderFactory
{
    public static IFaderStrategy MakeFader(FaderType type)
    {
        switch (type)
        {
            case FaderType.Hyperbolic:
                return new HyperbolicFader();
            case FaderType.Linear:
                return new LinearFader();
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
}

public class LinearFader : IFaderStrategy
{
    // Linearly fade away from 1
    private float _gradient;
    public LinearFader()
    {
        _gradient = PCGConfig.fadeGradient;
    }
    public float Fade(float distance)
    {
        return Mathf.Clamp(1.0f - (distance * _gradient), 0.0f, 1.0f);
    }
}

public class HyperbolicFader : IFaderStrategy
{
    // Ease out of 1 and ease into 0, but steep gradient through the middle
    public float Fade(float distance)
    {
        if (distance <= 0)
        {
            return 1.0f;
        } else if (distance >= 1)
        {
            return 0.0f;
        }

        float inverted_distance = 1 - distance;
        // The output function of this fader can be seen here https://www.desmos.com/calculator/hi3mksaava
        // and is constrained to the domain of [0,1]
        return 0.5f * MathF.Tanh(4 * inverted_distance - 2) + 0.5f; 
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
    public Mask(IFaderStrategy fader)
    {
        _fader = fader;
    }

    public NormalizingValues GetNormalizingValues()
    {
        return new NormalizingValues(0.0f, 1.0f);
    }

    public abstract float GeneratePixelValue(int x, int y);
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
    // A circular mask
    public RadialMask(IFaderStrategy fader) : base(fader){}

    public override float GeneratePixelValue(int x, int y)
    {
        float _radius = PCGConfig.radialMaskRadius;
        Vector2 _center = PCGConfig.radialMaskCenter;
        // Distance is calculated as a factor of the radius
        Vector2 coordinates = new Vector2(x, y);
        float distance = Vector2.Distance(coordinates, _center);
        float radius_proportional_distance = (distance / _radius) - 1.0f;
        return _fader.Fade(radius_proportional_distance);
    }
}

public class Generator
{
    public IGeneratorStrategy noiseGenerationStrategy;
    IGeneratorStrategy mask;
    public float[,] noiseMap = new float[0,0];


    
    
    public Generator( NoiseGeneratorType noiseType, MaskGeneratorType maskType, FaderType faderType)
    {
        noiseGenerationStrategy = GeneratorFactory.MakeNoiseGenerator(noiseType);
        IFaderStrategy fader = FaderFactory.MakeFader(faderType);
        mask = GeneratorFactory.MakeMaskGenerator(maskType, fader);
    }

    // Additional constructor using all default types. Likely won't be used in production but useful for testing
    public Generator()
    {
        noiseGenerationStrategy = GeneratorFactory.MakeNoiseGenerator(NoiseGeneratorType.Default);
        IFaderStrategy fader = FaderFactory.MakeFader(FaderType.Default);
        mask = GeneratorFactory.MakeMaskGenerator(MaskGeneratorType.Default, fader);
    }

    // Additional constructor to allow for custom setting of noise generators (mostly for debugging purposes)
    public Generator(MaskGeneratorType maskType, FaderType faderType)
    {
        IFaderStrategy fader = FaderFactory.MakeFader(faderType);
        mask = GeneratorFactory.MakeMaskGenerator(maskType, fader);
    }

    public void SetNoiseGenerator(IGeneratorStrategy gs)
    {
        noiseGenerationStrategy = gs;
    }

    public void VoidCache()
    {
        noiseMap = new float[0, 0];
    }

    public float[,] GenerateNoiseArray(Vector2 size)
    {
        // This function and the following create the noise array
        // This version takes a Vector2 to describe the canvas size

        // Use a cache first if one exists
        if (noiseMap.Length > 0)
        {
            return noiseMap;
        }
        
        int width = (int)size.x;
        int height = (int)size.y;
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
                container[j, i] *= mask.GeneratePixelValue(j, i);
            }
        }

        //Cache the created noiseMap
        noiseMap = container;
    }
}