#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-27
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

[CreateAssetMenu(menuName = "VFX/VFX Data")]
public class VFXData : ScriptableObject
{
    public GameObject[] Prefabs;
    public float Lifetime = 2.0f;
    public Vector3 spawnOffset;
    public GameObject GetRandomPrefab()
    {
        if (Prefabs == null || Prefabs.Length == 0) return null;
        return Prefabs[Random.Range(0,Prefabs.Length)];
    }
}