using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a service and is accesible through the service locator.
/// Monobehaviour wrapper for HazardManager
/// </summary>
public class HazardManagerBehaviour : MonoBehaviour
{
    public HazardGenerationStrategy.Strategy generationStrategy;

    private HazardManager _hazardManager;

    public HazardManager HazardManager
    {
        get
        {
            // Creates hazard manager object on first call
            if (_hazardManager == null)
            {
                TimeManager timeManager = ServiceLocator.Instance.GetService<TimeManagerBehavior>().TimeManager;
                GridManager gridManager = ServiceLocator.Instance.GetService<GridManagerBehaviour>().GridManager;

                _hazardManager = new HazardManager(timeManager, gridManager, null, generationStrategy);
            }

            return _hazardManager;
        }
    }

    private void Start()
    {
        HazardManager.Start();
    }

    private void OnEnable()
    {
        HazardManager.OnEnable();
    }

    private void OnDisable()
    {
        HazardManager.OnDisable();
    }

}
