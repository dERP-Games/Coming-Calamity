using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CLASS: VitalityStatsComponent
 * USAGE: Component that manages vital status for
 * creatures
 */
public class VitalityStatsComponent : MonoBehaviour
{
    // Private fields
    [SerializeField] float _health;
    [SerializeField] float _hunger;
    float _maxHealth = 100.0f;
    float _maxHunger = 100.0f;

    // Public properties
    public float Health
    {
        get { return _health; }
        set { _health = value; }
    }

    public float Hunger
    {
        get { return _hunger; }
        set { _hunger = value; }
    }

    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    public float MaxHunger
    {
        get { return _maxHunger; }
        set { _maxHunger = value; }
    }

    /// <summary>
    /// Adds to hunger stat
    /// </summary>
    /// <param name="hungerValue">Amount of hunger to fill</param>
    public void Feed(float hungerValue)
    {
        _hunger += hungerValue;
        _hunger = Mathf.Clamp(_hunger, _hunger, _maxHunger); // Clamp to max
    }

    /// <summary>
    /// Adds to health stat
    /// </summary>
    /// <param name="healValue">Amount to heal</param>
    public void Heal(float healValue)
    {
        _health += healValue;
        _health = Mathf.Clamp(_health, _health, _maxHealth); // Clamp to max
    }

    /// <summary>
    /// Depletes hunger
    /// </summary>
    /// <param name="hungerLost">Amount of hunger lost</param>
    public void ExpendEnergy(float hungerLost)
    {
        _hunger -= hungerLost;
    }

    /// <summary>
    /// Depletes health
    /// </summary>
    /// <param name="damage">Amount of health taken</param>
    public void TakeDamage(float damage)
    {
        _health -= damage;
    }
}
