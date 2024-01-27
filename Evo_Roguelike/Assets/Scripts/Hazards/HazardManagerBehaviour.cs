using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManagerBehaviour : MonoBehaviour
{
    public HazardGenerationStrategy.Strategy generationStrategy;

    private HazardManager _hazardManager;

    public HazardManager HazardManager
    {
        get
        {
            if (_hazardManager == null)
            {
                TimeManager timeManager = ServiceLocator.Instance.GetService<TimeManagerBehavior>().TimeManager;
                _hazardManager = new HazardManager(timeManager, generationStrategy);
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
