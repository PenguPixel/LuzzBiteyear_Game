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
using UnityEngine.Pool;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Projectile Configuration")]
    [SerializeField] private IntReference damageAmount;
    [SerializeField] private FloatReference moveSpeed;
    [SerializeField] private FloatReference maxLifetime;
    [SerializeField] private LayerMask targetLayers;
    
    #endregion
    #region Internal
    private IObjectPool<Projectile> parentPool;
    private float currentLifetime;
    private Transform targetTransform;
    #endregion

    
    #region Methods
    public void Initialize(IObjectPool<Projectile> pool, Vector3 spawnPosition, Quaternion spawnRotation, Transform target = null)
    {
        parentPool = pool;
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        targetTransform = target;
        currentLifetime = 0f;
    }

    private void Update()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= maxLifetime.Value)
        {
            ReleaseToPool();
            return;
        }

        if (targetTransform != null)
        {
            Vector3 dir = (targetTransform.position - transform.position).normalized;
            transform.position += dir * (moveSpeed.Value * Time.deltaTime);
        }
        else
        {
            transform.position += transform.forward * (moveSpeed.Value * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) == 0) return;

        if (other.TryGetComponent<Health>(out var health))
        {
            health.TakeDamage(damageAmount.Value);
        }

        ReleaseToPool();
    }
    #endregion


    #region Helpers
    private void ReleaseToPool()
    {
        if (parentPool != null)
            parentPool.Release(this);
        else
            gameObject.SetActive(false);
    }

    #endregion
}