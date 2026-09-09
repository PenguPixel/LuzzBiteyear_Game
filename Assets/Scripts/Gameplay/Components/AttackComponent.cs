#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-09
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
/// This class handles the attack/melee Action of an entity. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: [Describe what this class does].
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: [Where it should live in the Scene, Project, Assets'].
/// </summary>
#endregion


using UnityEngine;

[RequireComponent(typeof(TargetingComponent))]
[AddComponentMenu("Combat/Melee Attack Component")]
public class Attack : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif

    [Header("Dependencies")]
    [SerializeField] private TargetingComponent targetingComponent;
    [SerializeField] private MeleeAttack attackPayLoad;

    [Header("Events Configuration")]
    [SerializeField] private GameEvent onAttackExecuted; // maybe redundant
    
    #endregion


    #region Internal
    #endregion

    
    #region Methods
    public void ExecuteAttack()
    {
        if (onAttackExecuted != null) onAttackExecuted.Raise();

        if (attackPayLoad != null)
        {
            Transform target = targetingComponent != null ? targetingComponent.TargetTransform : null;
            attackPayLoad.ActivateHitbox(target);
        }
    }
    public void EndAttack()
    {
        if (attackPayLoad != null)
        {
            attackPayLoad.DeactivateHitbox();
        }
    }
    #endregion
}