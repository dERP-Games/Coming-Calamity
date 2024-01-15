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

    // Private fields
    Creature[] _Creatures;

    // Start is called before the first frame update
    void Start()
    {
        // Find all creatures currently in the scene
        _Creatures = GameObject.FindObjectsOfType<Creature>();

        // Spawn the initial population,
        // delayed to help prevent race conditions
        Invoke("SpawnInitialPopulation", 0.1f);
    }

    /*
	USAGE: Spawns the first set of creatures into the environment
	ARGUMENTS: ---
	OUTPUT: ---
	*/
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

    /*
	USAGE: Gets the next available creature from the pool to be spawned in
	ARGUMENTS: ---
	OUTPUT: int, the index of that creature in the creatures array
	*/
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

    /*
	USAGE: Spawns a creature into the environment and updates needed values
	ARGUMENTS:
    -	int creatureIndex -> location of creature reference in creatures array
	OUTPUT: ---
	*/
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

    /*
	USAGE: Finds a random location on the orthographic screen
	ARGUMENTS: ---
	OUTPUT: Vector2, randomly calculated location
	*/
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
}
