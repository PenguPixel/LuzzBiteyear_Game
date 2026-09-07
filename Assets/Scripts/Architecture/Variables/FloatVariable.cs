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
/// This class handles the data structure for float variables. It must maintain modular design. It is not overengineering.
/// </para>
/// </remarks>
/// <summary>
/// Description: Dataholder for Variables that need to be available globally.
/// Coordination: Lives in the system. Use FloatReference and assign required variable.
/// Deployment: Create Assets for Variables that require global availability. For example player health.
/// </summary>
#endregion


using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Variables/Float")]
public class FloatVariable : ScriptableObject
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif    
    #endregion
    [SerializeField] private float value;
    public float Value => value;

#region Modifications
    public void ApplyChange(float amount)
    {
        value += amount;
    }
    public void SetValue(float newValue)
    {
        value = newValue ;
    }
#endregion
}

[Serializable]
public class FloatReference
{
    public bool UseConstant = false;
    public float ConstantValue;
    public FloatVariable Variable;

    public float Value => UseConstant ? ConstantValue : Variable.Value ;
}