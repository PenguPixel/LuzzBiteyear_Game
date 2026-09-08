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
/// This class handles Vector3 data. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Like all other variables.
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: [Where it should live in the Scene, Project, Assets'].
/// </summary>
#endregion


using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Variables/Vector3")]
public class Vector3Variable : ScriptableObject
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] private Vector3 value;
    
    #endregion
    #region Internal
    public Vector3 Value => value;
    #endregion
    public void SetValue(Vector3 newValue)
    {
        value = newValue;
    }
}

[Serializable]
public class Vector3Reference
{
    [SerializeField] private bool useConstant;
    [SerializeField] private Vector3 constantValue;
    [SerializeField] private Vector3Variable variable;

    public Vector3 Value
    {
        get => useConstant ? constantValue : (variable != null ? variable.Value : Vector3.zero);
        set
        {
            if (useConstant)
            {
                constantValue = value;
            }
            else if(variable != null)
            {
                variable.SetValue(value);
            }
        }        
    }
}