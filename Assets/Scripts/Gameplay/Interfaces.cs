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
/// This class handles holds Interfaces relevant to the Game. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: it defines interfaces that we require.
/// Coordination: They are in fact interfaces.
/// Deployment: Scripts/Gameplay.
/// </summary>
#endregion


using UnityEngine;

#region Generalized
public interface IChangeValue<T>
{
    void ApplyChange(T amount);
}
public interface ISetValue<T>
{
    void SetValue(T newValue);
}
public interface IInteractable
{
    InteractionData InteractionData { get; }
    void Interact(GameObject interactor);
}
#endregion


#region Specified
public interface IDamageable
{
    void TakeDamage(int amount, GameObject damageSource);
}
public interface ITargetable
{
    Transform TargetTransform { get; }
    bool IsTargetable { get; }
}
public interface IHealable
{
    void Heal(int amount, GameObject healSource);
}
public interface IUseEnergy
{
    void UseEnergy(int amount, GameObject drainSource);
}
public interface IReplenishEnergy
{
    void ReplenishEnergy(int amount, GameObject chargeSource);
}
#endregion