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
/// This class handles the health of the entitity it is attached to. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Holds Data to the health of an entity.
/// Coordination: Reacts to appropriate events by Damaging or healing the player.
/// Deployment: Attach a health Component to an entity.
/// </summary>
#endregion


using System;
using UnityEngine;

[AddComponentMenu("Resources/Health Resource")]
public class Health : MonoBehaviour, IDamageable, IHealable
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Health Data")]
    [SerializeField] private IntReference currentHealth;
    [SerializeField] private IntReference maxHealth;

    [Header("Events")]
    [SerializeField] private GameEvent onHealthChanged;
    [SerializeField] private GameEvent onDied;

    #endregion


    #region Internal
    #endregion


    #region Methods
    public void TakeDamage(int amount, GameObject damageSource = null)
    {
        if (currentHealth.Value <= 0) return;
        currentHealth.Value -= amount;
        if (onHealthChanged != null)
            onHealthChanged.Raise();
        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }
    public void Heal(int amount, GameObject healSource = null)
    {
        if (currentHealth.Value >= maxHealth.Value) return;
        currentHealth.Value = Math.Clamp(currentHealth.Value + amount, 0, maxHealth.Value);
        if (onHealthChanged != null)
            onHealthChanged.Raise();
    }
    public void Die()
    {
        if (onDied != null)
            onDied.Raise();
    }
    #endregion
}