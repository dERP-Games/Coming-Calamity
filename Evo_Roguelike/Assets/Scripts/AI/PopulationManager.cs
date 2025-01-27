using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CLASS: PopulationManager
 * USAGE: Manager for creatures which handles population stats, 
 * spawning, and pooling of Creature objects.
 */
public class PopulationManager : MonoBehaviour
{
    // Public fields
    public int initialSpawnCount = 5;
    public VitalityStatsSetup populationVitalityRanges;

    // Private fields
    Creature[] _Creatures;

    // Public field for getting species stat manager
    public SpeciesStatsManager StatsManager
    {
        get { return GetComponent<SpeciesStatsManager>(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Find all creatures currently in the scene
        _Creatures = GameObject.FindObjectsOfType<Creature>();

        // Spawn the initial population,
        // delayed to help prevent race conditions
        Invoke("SpawnInitialPopulation", 0.1f);

        // Initialize population vital stats
        InitializePopulationVitals();
    }

    /// <summary>
    /// Spawns the first set of creatures into the environment
    /// </summary>
    void SpawnInitialPopulation()
    {
        // Check list isn't empty
        if (_Creatures.Length > 0)
        {
            // Iterate through and spawn the first set of targets
            for (int i = 0; i < initialSpawnCount; i++)
            {
                int spawnIndex = GetNextAvailableCreature();
                if (spawnIndex >= 0)
                {
                    SpawnCreature(spawnIndex);
                }
            }
        }
    }

    /// <summary>
    /// Gets the next available creature from the pool to be spawned in
    /// </summary>
    /// <returns>int, the index of that creature in the creatures array</returns>
    int GetNextAvailableCreature()
    {
        // Iterate through the creatures until you
        // find one that isn't already active
        for (int i = 0; i < _Creatures.Length; i++)
        {
            // Check if on screen
            if (!_Creatures[i].bIsActive)
            {
                // Return index where creature is located
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Spawns a creature into the environment and updates needed values
    /// </summary>
    /// <param name="creatureIndex">Location of creature reference in creatures array</param>
    void SpawnCreature(int creatureIndex)
    {
        // Get creature from index
        Creature creature = _Creatures[creatureIndex];

        // Get the spawn point randomly and teleport the creature to that point
        Vector3 spawnPoint = GetRandomPointOnScreen();
        creature.transform.position = spawnPoint;

        // Update on-screen status
        creature.bIsActive = true;
        creature.StateMachine.TransitionToState(CreatureStateMachine.CreatureStates.Wandering);
    }

    /// <summary>
    /// Finds a random location on the orthographic screen
    /// </summary>
    /// <returns>Vector2, Randomly calculated location</returns>
    Vector2 GetRandomPointOnScreen()
    {
        // Get max height and width values from screen
        float maxHeight = Camera.main.GetComponent<Camera>().orthographicSize;
        float maxWidth = maxHeight * (Screen.width / Screen.height);

        // Get a random range for x and y levels
        float newPosX = Random.Range((-maxWidth * 2), (maxWidth * 2));
        float newPosY = Random.Range(-maxHeight, maxHeight);

        return new Vector2(newPosX, newPosY);
    }


    /// <summary>
    /// Initializes vitality stats for each creature in population
    /// </summary>
    void InitializePopulationVitals()
    {
        // Get config for vitality stats
        List<VitalityStat> statConfigs = populationVitalityRanges.statConfigs;

        // Iterate through each creature setting their initial stat values
        foreach(Creature creature in _Creatures)
        {
            VitalityStatsComponent vitals = creature.VitalityStats;

            // Go through each stat in config and set stats per type
            foreach(VitalityStat stat in statConfigs)
            {
                switch (stat.statType)
                {
                    case VitalityStatType.Health:
                        vitals.MaxHealth = HelperFunctions.Gaussian(stat.medianValue, stat.stdDev);
                        vitals.Health = vitals.MaxHealth;
                        break;
                    case VitalityStatType.Hunger:
                        vitals.MaxHunger = HelperFunctions.Gaussian(stat.medianValue, stat.stdDev);
                        vitals.Hunger = vitals.MaxHunger;
                        break;
                }

            }
        }
    }
}
