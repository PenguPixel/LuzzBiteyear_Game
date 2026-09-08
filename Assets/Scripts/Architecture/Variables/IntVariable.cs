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
/// This class handles the data structure for integer variables. It must maintain modular design. It is not overengineering.
/// </para>
/// </remarks>
/// <summary>
/// Description: Dataholder for Variables that need to be available globally.
/// Coordination: Lives in the system. Use FloatReference and assign required variable.
/// Deployment: Create Assets for Variables that require global availability. For example player health./// </summary>
#endregion


using UnityEngine;
using System;

[CreateAssetMenu(menuName ="Variables/Integer")]
public class IntVariable : ScriptableObject
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif    
    [SerializeField] private int value;
    #endregion
    
    public int Value => value;
#region Modifications
    public void ApplyChange(int amount)
    {
        value += amount;
    }
    public void SetValue(int newValue)
    {
        value = newValue ;
    }
#endregion
    
}

[Serializable]
public class IntReference
{
    [SerializeField] private bool useConstant = true;
    [SerializeField] private int constantValue;
    [SerializeField] private IntVariable variable;

    public int Value
    {
        get => useConstant ? constantValue : variable.Value;
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