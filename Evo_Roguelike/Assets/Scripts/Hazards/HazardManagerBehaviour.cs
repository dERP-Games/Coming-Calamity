using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManagerBehaviour : MonoBehaviour
{
    private HazardManager _hazardManager;

    public HazardManager HazardManager
    {
        get
        {
            if (_hazardManager == null)
            {
                _hazardManager = new HazardManager(ServiceLocator.Instance.GetService<TimeManagerBehavior>().TimeManager);
            }

            return _hazardManager;
        }
    }

    private void Start()
    {
        if (_hazardManager == null)
        {
            _hazardManager = new HazardManager(ServiceLocator.Instance.GetService<TimeManagerBehavior>().TimeManager);
        }
    }
}
