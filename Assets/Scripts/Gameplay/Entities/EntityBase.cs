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

[RequireComponent(typeof(Health))]
[AddComponentMenu("Entities/Base Entity")]
public class EntityBase : MonoBehaviour, ITargetable
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Events")]
    [SerializeField] private GameEvent onEntitySpawned;
    [SerializeField] private GameEvent onEntityDied;
    
    #endregion


    #region Internal
    private PoolManager poolManager;
    private GameObject sourcePrefab;
    public Transform TargetTransform => transform;

    public bool IsTargetable => gameObject.activeInHierarchy;

    private void OnDisable()
    {
        if (onEntityDied != null)
            onEntityDied.Raise();
    }
    #endregion


    #region Methods
    public void InitializeEntity(PoolManager manager, GameObject prefab)
    {
        poolManager = manager;
        sourcePrefab = prefab;

        if (onEntitySpawned != null)
            onEntitySpawned.Raise();
    }
    public void OnDeath()
    {
        if (poolManager != null && sourcePrefab != null)
        {
            poolManager.Release(sourcePrefab, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    #endregion
}