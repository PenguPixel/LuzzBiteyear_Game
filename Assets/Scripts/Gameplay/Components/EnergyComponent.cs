#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-08
*/
#endregion


#region Basic Instruction
/*
Structure the class into regions as appropriate for their use case.
The regions should separate what is viewed or used in an inspector, class intern relevant fields, Public Getters if necessary, 
Use top comments above methods to describe them and explain their parameters.
TODO comments above a method or codeblock
Use side comments in line to describe lines that obfuscate their function as explanation
*/
#endregion


#region Development remarks
/// <remarks>
/// <para>
/// This class handles the Energy Managemnet of an entity. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: It holds the reference to the data holders if local values are not assigned. Provides functions to be called to manipulate Energy data and raise events.
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: Can be added at will to GameObjects.
/// </summary>
#endregion


using System;
using UnityEngine;


[AddComponentMenu("Resources/Energy Resource")]
public class Energy : MonoBehaviour, IUseEnergy, IReplenishEnergy
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Energy Data")]
    [SerializeField] private IntReference currentEnergy;
    [SerializeField] private IntReference maxEnergy;

    [Header("Energy Events")]
    [SerializeField] private GameEvent onEnergyChanged;
    
    #endregion


    #region Methods
    public void ReplenishEnergy(int amount, GameObject chargeSource)
    {
        if (currentEnergy.Value >= maxEnergy.Value) return;
        currentEnergy.Value += Math.Clamp(amount, 0, maxEnergy.Value) ;
        if (onEnergyChanged != null)
            onEnergyChanged.Raise();
    }

    public void UseEnergy(int amount, GameObject drainSource)
    {
        if (currentEnergy.Value <= 0) return;
        currentEnergy.Value = Math.Clamp(currentEnergy.Value - amount, 0, maxEnergy.Value);
        if (onEnergyChanged != null)
            onEnergyChanged.Raise();
    }

    #endregion
}