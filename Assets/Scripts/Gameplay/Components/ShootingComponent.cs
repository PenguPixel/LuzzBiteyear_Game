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


using System;
using UnityEngine;


[RequireComponent(typeof(TargetingComponent))]
[AddComponentMenu("Combat/Shooting Component")]
public class ShootingComponent : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Dependencies")]
    [SerializeField] private TargetingComponent targetingComponent;
    [SerializeField] private PoolManager poolManager;
    [SerializeField] private Energy energy;

    [Header("Weapon Configuration")]
    [SerializeField] private FloatReference attackRange;
    [SerializeField] private FloatReference cooldownTime;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;


    #endregion


    #region Internal
    private float lastTimeFire;
    private Func<Vector3, Quaternion, GameObject> spawnDelegate;
    private void Start()
    {
        if (poolManager != null && projectilePrefab != null)
        {
            poolManager.Prewarm(projectilePrefab, 10);
            spawnDelegate = poolManager.GetSpawnDelegate(projectilePrefab);
        }
    }
    #endregion


    #region Public Getters
    public float AttackRange => attackRange != null ? attackRange.Value : 0f;
    public bool IsInAttackRange(Vector3 origin, Vector3 targetPos) => Vector3.Distance(origin, targetPos) <= AttackRange;
    public bool CanFire => Time.time >= lastTimeFire + (cooldownTime != null ? cooldownTime.Value : 1f);

    #endregion

    #region Methods
    public void ExecuteFire()
    {
        if (energy != null)
            energy.UseEnergy(1, null);
        if (!CanFire) return;
        lastTimeFire = Time.time;

        Transform origin = firePoint != null ? firePoint : transform;
        Quaternion spawnRotation = origin.rotation; // default fallback, in case the targeting is just chanigng in any way.

        if (targetingComponent != null && targetingComponent.HasValidTarget)
        {
            Vector3 targetDir = (targetingComponent.TargetTransform.position - origin.position).normalized;
            if (targetDir != Vector3.zero)
            {
                spawnRotation = Quaternion.LookRotation(targetDir);
            }
        }

        if (spawnDelegate != null)
        {
            GameObject projectile = spawnDelegate.Invoke(origin.position, spawnRotation);
            if (projectile.TryGetComponent<Projectile>(out var payload))
            {
                Transform target = targetingComponent != null ? targetingComponent.TargetTransform : null;
                payload.Initialize(poolManager, projectilePrefab, target);
            }
        }
    }
    #endregion
}