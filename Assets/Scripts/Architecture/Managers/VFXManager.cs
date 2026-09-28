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

using System.Collections;
using UnityEngine;


public class VFXManager : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] private VFXEventChannel vfxEventChannel;
    [SerializeField] private PoolManager poolManager;
    #endregion


    #region Internal
    private void OnEnable()
    {
        if (vfxEventChannel != null)
            vfxEventChannel.OnVFXRequested += HandleVFXRequest;
    }
    private void OnDisable()
    {
        if (vfxEventChannel != null)
            vfxEventChannel.OnVFXRequested -= HandleVFXRequest;
        
    }
    #endregion

    
    #region Methods
    private void HandleVFXRequest(VFXData data, Vector3 position, Quaternion rotation)
    {
        if (data == null || poolManager == null) return;
        GameObject prefab = data.GetRandomPrefab();
        if (prefab == null) return;

        var spawnDelegate = poolManager.GetSpawnDelegate(prefab);
        if (spawnDelegate == null) return;

        GameObject vfxInstance = spawnDelegate(position, rotation);
        StartCoroutine(RecycleRoutine(prefab, vfxInstance, data.Lifetime));
    }
    private IEnumerator RecycleRoutine(GameObject prefab, GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (instance != null && instance.activeSelf)
            poolManager.Release(prefab, instance);
    }
    #endregion
}