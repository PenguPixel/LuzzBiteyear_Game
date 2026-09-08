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
/// This class handles [Core Responsibility]. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: [Describe what this class does].
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: [Where it should live in the Scene, Project, Assets'].
/// </summary>
#endregion


using UnityEngine;


public class Health : MonoBehaviour, IDamageable
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Health Data")]
    [SerializeField] private IntReference currentHealth;
    [SerializeField] private IntReference maxHealth;

    [Header("Events")]
    [SerializeField] private GameEvent onDamaged;
    [SerializeField] private GameEvent onDied;

    #endregion
    #region Internal
    private void Start()
    {
        if (currentHealth != null && maxHealth != null)
        {
            currentHealth.Value = maxHealth.Value;
        }
    }
    #endregion


    #region Methods
    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    public void GoodMethod(int parameters)
    {
        /* --- CodeBlock: Logic Execution --- */
        // Description: Describe the intent of this specific block
        var value = parameters * 2;   // Descriptive comment for specific line, if necessary
    }

    public void TakeDamage(int amount, GameObject damageSource = null)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}