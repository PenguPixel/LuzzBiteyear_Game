#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-29
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

[RequireComponent(typeof(Rigidbody))]
public class Tumbleweed : MonoBehaviour
{
    #region Inspector
    [Header("Wind Direction & Multipliers")]
    [SerializeField] private Vector3 baseWindDirection = new Vector3(1f, 0f, 0.5f);
    [SerializeField] private float forceMultiplier = 3f;
    #endregion


    #region Internal
    private Rigidbody rb;
    private Terrain activeTerrain;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        baseWindDirection = baseWindDirection.normalized;
    }

    private void Start()
    {
        activeTerrain = Terrain.activeTerrain;
    }

    private void FixedUpdate()
    {
        if (activeTerrain == null || activeTerrain.terrainData == null) return;

        TerrainData data = activeTerrain.terrainData;
        float grassSpeed = data.wavingGrassSpeed;
        float grassStrength = data.wavingGrassStrength;

        float pulse = Mathf.Sin(Time.time * grassSpeed) * grassStrength;
        float totalForce = (grassStrength + pulse) * forceMultiplier;

        rb.AddForce(baseWindDirection * totalForce, ForceMode.Force);
    }
   #endregion
}