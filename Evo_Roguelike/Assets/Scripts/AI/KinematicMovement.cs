using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CLASS: KinematicMovement
 * USAGE: Component used for Creature movement and holds all methods needed for
 * managing different movement scenarios.
 */
public class KinematicMovement : MonoBehaviour
{
    // Public fields
    public Vector2 targetPosition;
    public float targetRadius;
    public LayerMask creatureMask;

    // Maximums
    [Range(0, 10)]
    public float maxSpeed = 3f;

    [Range(.1f, .5f)]
    public float maxForce = .03f;

    // Separation fields
    [Range(0.1f, 5.0f)]
    public float flockSize = 3f;

    [Range(0.0f, 5.0f)]
    public float separationStrength = 1f;

    // Private fields
    private bool _bCanMove;
    private bool _bIsWandering;
    private Vector2 _CurrentVelocity;
    private SpriteRenderer _SpriteRenderer;

    // Public properties
    public bool CanMove
    {
        // Get/Set flag
        get { return _bCanMove; }
        set { _bCanMove = value; }
    }

    public bool IsWandering
    {
        // Get/Set flag
        get { return _bIsWandering; }
        set { _bIsWandering = value; }
    }

    // Ran at beginning of game
    void Start()
    {
        // Init components
        _SpriteRenderer = GetComponent<SpriteRenderer>();

        // Set the first wander position
        SetWanderPosition();
    }

    // Update is called once per frame
    void Update()
    {
        // Only steer if agent is currently moveable
        if (_bCanMove)
        {
            // Check if target reaches destination in wander state
            if (IsAtDestination() && _bIsWandering)
            {
                // Find new position once at destination
                SetWanderPosition();
                return;
            }

            // Agents stay separated from each other
            // and steer away when close
            ApplySeparation();
            UpdateVelocity();
            UpdatePosition();
        }
    }

    /*
	USAGE: Updates the agent position based on the current velocity
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    void UpdatePosition()
    {
        // Calculate moving velocity
        Vector2 finalVelocity = _CurrentVelocity.normalized * maxSpeed;

        // Update position based on final velocity
        transform.position += new Vector3(finalVelocity.x, finalVelocity.y, 0.0f) * Time.deltaTime;
    }

    /*
	USAGE: Updates the agent's velocity to steer towards target position
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    void UpdateVelocity()
    {
        // Desired velocity for target position
        Vector2 desiredVelocity = targetPosition - new Vector2(transform.position.x, transform.position.y);

        // Steer agent to the wander point
        _CurrentVelocity += Steer(desiredVelocity.normalized * maxSpeed);
        _CurrentVelocity = LimitMagnitude(_CurrentVelocity, maxSpeed);
    }

    /*
	USAGE: Returns a steering velocity calculated to reach desired velocity
	ARGUMENTS:
    -	Vector2 desired -> desired velocity
	OUTPUT: Vector2, Steering velocity to get closer to desired velocity
	*/
    Vector2 Steer(Vector2 desired)
    {
        Vector2 steer = desired - _CurrentVelocity;
        steer = LimitMagnitude(steer, maxForce);

        return steer;
    }

    /*
	USAGE: Sets the agent's target position
	ARGUMENTS:
    -	Vector2 newTarget -> new target location
	OUTPUT: ---
	*/
    public void SetTargetPosition(Vector2 newTarget)
    {
        // Set the position
        targetPosition = newTarget;
    }

    /*
	USAGE: Sets target position to random location on the screen, used for free wandering
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    public void SetWanderPosition()
    {
        // Get max heigh and width values from screen
        float maxHeight = Camera.main.GetComponent<Camera>().orthographicSize;
        float maxWidth = maxHeight * (Screen.width / Screen.height);

        // Get a random range for x and y levels
        float newPosX = Random.Range((-maxWidth * 2) + _SpriteRenderer.size.x, (maxWidth * 2) - _SpriteRenderer.size.x);
        float newPosY = Random.Range(-maxHeight + _SpriteRenderer.size.y, maxHeight - _SpriteRenderer.size.y);

        // Set the new position
        SetTargetPosition(new Vector2(newPosX, newPosY));
    }

    /*
	USAGE: Checks whether or not agent has reached their destination
	ARGUMENTS: ---
	OUTPUT: boolean, whether or not they made it
	*/
    public bool IsAtDestination()
    {
        // Get the direction towards target position
        Vector2 direction = targetPosition - new Vector2(transform.position.x, transform.position.y);
        float distance = direction.magnitude;

        // Return result
        return distance <= targetRadius;
    }

    /*
	USAGE: Applies velocity calculated from separation algorithm to agent's current velocity
	ARGUMENTS: ---
	OUTPUT: ---
	*/
    void ApplySeparation()
    {
        // Get all nearby neighbors and apply separation algorithm to velocity
        Collider2D[] nearbyCreatures = Physics2D.OverlapCircleAll(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), flockSize, creatureMask);
        _CurrentVelocity += Separate(nearbyCreatures) * separationStrength;
    }

    /*
	USAGE: Calculates velocity needed to separate agent from other nearby agents
	ARGUMENTS: 
    -	Collider2D[] neighbors -> array of all nearby agent colliders
	OUTPUT: Vector2, calculated steering velocity
	*/
    Vector2 Separate(Collider2D[] neighbors)
    {
        // Check that neighbors arent empty
        if (neighbors.Length == 0)
            return Vector2.zero;

        // Init direction to default vector
        Vector2 direction = Vector2.zero;

        // Iterate through each neighbor
        foreach (Collider2D neighbor in neighbors)
        {
            // Get creature component from neighbor
            Creature curCreature = neighbor.GetComponent<Creature>();

            // Continue if the bat isnt on screen
            if (!curCreature.bIsActive)
                continue;

            // Continue if the bat isnt in the flock radius
            if (Vector3.Distance(transform.position, neighbor.transform.position) > flockSize)
                continue;

            // Calculate a vector pointing to the neighbor
            Vector2 difference = transform.position - neighbor.transform.position;

            // Calculate new direction by dividing by the distance
            direction += difference.normalized / difference.magnitude;
        }

        // Divide the direction by total number of neighbors
        direction /= neighbors.Length;

        // Return desired velocity from steer
        return Steer(direction.normalized * maxSpeed);
    }

    /*
	USAGE: Limits magnitude of a given vector
	ARGUMENTS: 
    -	Vector2 baseVector -> vector to clamp to the max
    -	float maxMagnitude -> clamp value of vector magnitude
	OUTPUT: Vector2, clamped vector
	*/
    Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
    {
        // If magnitude is larger than the max, clamp it to the max
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }
        return baseVector;
    }
}
