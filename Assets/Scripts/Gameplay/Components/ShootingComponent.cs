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
/// This class handles shooting a device or object incorporated into the Actor. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Shoots something at a target.
/// Coordination: Gets the call by its controller. Assess the target from the targeting component. Calls the PoolManager to fire something at the target from its location
/// Deployment: Component on entity.
/// </summary>
#endregion


using System;
using UnityEngine;


[RequireComponent(typeof(TargetingComponent))]
[RequireComponent(typeof(CharacterAnimationBridge))]
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
    [Tooltip("Optional: If assigned on Player, firing consumes Energy. If null on Enemy, only cooldown applies.")]
    [SerializeField] private Energy energy;
    [SerializeField] private CharacterAnimationBridge animationBridge;

    [Header("Weapon Configuration")]
    [SerializeField] private FloatReference attackRange;
    [SerializeField] private FloatReference cooldownTime;
    [SerializeField] private int energyCost = 1;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Audio")]
    [SerializeField] private AudioEventChannel audioChannel;
    [SerializeField] private SoundData shotSO;
    #endregion


    #region Internal
    private float _lastTimeFired;
    private Func<Vector3, Quaternion, GameObject> _spawnDelegate;

    private void Awake()
    {
        if (targetingComponent == null)
        {
            targetingComponent = GetComponent<TargetingComponent>();
        }

        if ( energy == null)
        {
            energy = GetComponent<Energy>();
        }
    }
    private void Start()
    {
        if (poolManager == null) poolManager = FindFirstObjectByType<PoolManager>();
        if (poolManager != null && projectilePrefab != null)
        {
            poolManager.Prewarm(projectilePrefab, 10);
            _spawnDelegate = poolManager.GetSpawnDelegate(projectilePrefab);
        }
        if (animationBridge == null) animationBridge = GetComponent<CharacterAnimationBridge>();
    }
    private void OnEnable()
    {
        if (animationBridge != null)
            animationBridge.OnShootFrame += SpawnProjectile;
    }
    private void OnDisable()
    {
        if (animationBridge != null)
            animationBridge.OnShootFrame -= SpawnProjectile;
    }
    #endregion


    #region Public Getters
    public float AttackRange => attackRange != null ? attackRange.Value : 0f;
    public bool IsInAttackRange(Vector3 origin, Vector3 targetPos) => Vector3.Distance(origin, targetPos) <= AttackRange;
    public float CooldownDuration => cooldownTime != null ? cooldownTime.Value : 0.5f;
    public bool IsCooldownReady => Time.time >= _lastTimeFired + CooldownDuration;
    public bool HasRequiredEnergy => energy == null || energyCost <= 0 || energy.HasEnergy(energyCost);
    public bool CanFire => IsCooldownReady && HasRequiredEnergy;

    #endregion

    #region Methods
    public bool ExecuteFire()
    {
        if (!CanFire) return false;
        if (!targetingComponent.HasValidTarget) return false;

        if (energy != null && energyCost > 0)
        {
            energy.UseEnergy(energyCost, gameObject);
        }

        _lastTimeFired = Time.time;

        if (energy != null)
            energy.UseEnergy(1, null);

        if (animationBridge != null)
            animationBridge.TriggerShoot();
        return true;
    }
    private void SpawnProjectile()
    {
        Transform origin = firePoint != null ? firePoint : transform;
        Quaternion spawnRotation = origin.rotation; // default fallback, in case the targeting is just chanigng in any way.

        if (targetingComponent != null && targetingComponent.HasValidTarget)
        {
            Vector3 targetDir = (targetingComponent.TargetTransform.position - origin.position).normalized;
            if (targetDir != Vector3.zero)            
                spawnRotation = Quaternion.LookRotation(targetDir);
            
        }

        if (_spawnDelegate != null)
        {
            GameObject projectile = _spawnDelegate.Invoke(origin.position, spawnRotation);
            if (projectile.TryGetComponent<Projectile>(out var payload))
            {
                Transform target = targetingComponent != null ? targetingComponent.TargetTransform : null;
                LayerMask mask = targetingComponent != null ? targetingComponent.TargetLayerMask : default;
                payload.Initialize(poolManager, projectilePrefab, target, mask);
            }
        }

        if (audioChannel != null && shotSO != null)
            audioChannel.RaiseSFX(shotSO, origin.position);
    }
    #endregion
}
