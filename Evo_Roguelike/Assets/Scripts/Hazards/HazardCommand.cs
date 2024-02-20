using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for all hazard commands.
/// </summary>
public abstract class HazardCommand
{
    public int timestampToStart;

    protected GridManager _gridManager;
    protected PopulationManager _populationManager;

    public delegate void DHazardExecuted();
    public DHazardExecuted dHazardExecuted;

    public HazardCommand(int timestampToStart, GridManager gridManager, PopulationManager populationManager)
    {
        this.timestampToStart = timestampToStart;
        _gridManager = gridManager;
        _populationManager = populationManager;
    }

    /// <summary>
    /// Details effect of hazard on environment
    /// </summary>
    public abstract void EnvironmentEffect();

    /// <summary>
    /// Details effect of hazard on population
    /// </summary>
    public abstract void PopulationEffect();

    /// <summary>
    /// Executes both environment and population effects
    /// </summary>
    public void ExecuteEffects()
    {
        EnvironmentEffect();
        PopulationEffect();

        dHazardExecuted?.Invoke();

        // Get UI Manager and tell it to enable traits panel
        ServiceLocator.Instance.GetService<UIManager>().EnableTraitsGUI();
    }
}
