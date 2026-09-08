#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-07
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
/// This class handles automated subscription service of GOs to a specific RuntimeSet. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Attached to any prefab that dynamically changes its existence in a scene. Configure its target Runtime Set
/// Coordination: Automatically handles register and unregister of the object to the target Set.
/// Deployment: Attatch this to any prefab that should register itself automatically when it enters or leaves a scene.
/// </summary>
#endregion


using UnityEngine;

public class GameObjectSetElement : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] private GameObjectSet targetSet;

    #endregion
    #region Internal
    private void OnEnable()
    {
        if (targetSet != null) targetSet.Add(gameObject);
    }
    private void OnDisable()
    {
        if (targetSet != null) targetSet.Remove(gameObject);        
    }
    #endregion
}