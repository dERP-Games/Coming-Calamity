using UnityEngine;

public static class WeatherService
{
    //Types of precipitation for when we want to implement them into the game
    public enum PrecipitationType
    {
        None,
        Rain,
        Snow
    }

    /* Function to calculate a temperature value. It oscillates around 0 and can, in theory,
     * take any value from -infinity to positive infinity. However, given that the output
     * is calculated using an inverse power rule, it takes a long time to exceed the bounds
     * of [-10,10]. 
     * The sine wave has been offset and phase shifted so that every turn hits close to a maximum
     * or a minimum of a wave.
    
     Inputs:
        - timeMarker: A turn in this game
        - bias: Temperature bias to account for the terrain being located in a warm or cold region of Earth
        - boundExpansion: The value of the output of the bound expansion formula. This is passed as a parameter to calculate it once and save compute time
        - height: The height of the terrain for which the temperature is being calculated
     Output:
        - A float that is added to the random temperature mean for the island
     */
    public static float CalculateTemperature(int timeMarker, float bias, float boundExpansion, float height)
    {
        float heightSubtraction = height * ServiceLocator.Instance.GetService<TerrainGenerationManager>().heightTemperatureSlope;
        return boundExpansion * Mathf.Sin(Mathf.PI * timeMarker + Mathf.PI*0.5f) + bias - heightSubtraction;
    }

    /*
     Function to calculate how much the maximum and minimum temperature deltas have changed
    since the beginning of the game. This takes the turn count as input and outputs the
    attenuation or augmentation factor produced by this calculation.

    It is an inverse power formula, meaning the output increases very slowly as the input increases.
     */
    public static float CalculateTemperatureBoundExpansion(int timeMarker)
    {

        return Mathf.Pow(0.1f*timeMarker, 0.25f);
    }

    /*
     The formula for calculating the humidity output of the current turn. Humidity decreases
    with height and is independent of temperature. Outputs are bounded to [0,1]
     */
    public static float CalculateHumidity(int timeMarker, float height)
    {
        TerrainGenerationManager tgm = ServiceLocator.Instance.GetService<TerrainGenerationManager>();
        float heightSubtraction = height * tgm.heightTemperatureSlope;
        float phaseShift = 2 * timeMarker + tgm.humidityPhaseShift;
        return Mathf.Clamp01(0.5f+0.5f*Mathf.Sin(phaseShift) - heightSubtraction);
    }

    /*
     Calculates the probability of precipitation in a given tile. Current formula
    is to treat temperature as symmetric function where towards being temperate probability is low
    but too hot or too cold can lead to increased precipitation. Also, humidity plays a linear factor.
    */
    public static float CalculatePrecipitationChance(float temperature, float humidity)
    {
        return .75f * Mathf.Abs(0.5f * temperature) * humidity;
    }
}
