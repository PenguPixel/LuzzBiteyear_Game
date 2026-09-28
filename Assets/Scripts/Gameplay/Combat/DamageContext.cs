#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-16
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
#endregion


using UnityEngine;

/// <remarks>
/// <para>
/// This class handles a struct that is supposed to be available for damage soruces. Since we wanted to add a heal meahcanic7
/// I thought of a way to pass the information of Payloads on.
/// </para>
/// </remarks>
/// <summary>
/// Description: Serves as a struct for payloads or attacks in general to pass on information to the affected.
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: jsut a script.
/// </summary>
public enum DamageType
{
    Generic,
    Melee,
    Ranged,
    Environmental
}

public struct DamageContext
{
    public int Amount;
    public DamageType Type;
    public GameObject Source;
    public DamageContext(int amount, DamageType type, GameObject source)
    {
        Amount = amount;
        Type = type;
        Source = source;
    }
}