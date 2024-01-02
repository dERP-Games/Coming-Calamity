using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is a Singleton and is responsible for controlling the access to the different services in the game.
 * This script needs to be attached on the parent gameobject to all the services.
 */
public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance { get; private set; }

    private Dictionary<Type, MonoBehaviour> servicesDict;

    private void Awake()
    { 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        servicesDict = new Dictionary<Type, MonoBehaviour>();
    }

    /*
     * A templated function that gets a Monobehavior service that is a children of this game object.
     * Output
     * T: Monobehavior component requested.
     */
    public T GetService<T>() where T: MonoBehaviour
    {
        if (servicesDict == null) return null;

        if(servicesDict.ContainsKey(typeof(T)))
        {
            return (T) servicesDict[typeof(T)];
        }
        else
        {
            T component = GetComponentInChildren<T>();
            if(!component) return null;

            servicesDict.Add(typeof(T), component);
            return (T)servicesDict[typeof(T)];
        }
        
    }
}
