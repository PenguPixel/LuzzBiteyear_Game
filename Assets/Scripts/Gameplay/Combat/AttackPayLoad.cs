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

[RequireComponent(typeof(Collider))]
public class MeleeAttack : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Attack Configuration")]
    [SerializeField] private IntReference damageAmount;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private Collider hitBoxCollider;

    #endregion


    #region Internal
    private void Awake()
    {
        if (hitBoxCollider == null) hitBoxCollider = GetComponent<Collider>();
        hitBoxCollider.enabled = false;
    }
    #endregion


    #region Methods
    /// <summary>
    /// HitDetection of designated Opponent Layers. OnHit deals designated damage
    /// </summary>
    /// <param name="other">object hit by this</param>
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) == 0) return;
        if (other.TryGetComponent<Health>(out var health))
        {
            health.TakeDamage(damageAmount.Value);
        }
    }
    #endregion


    #region Helpers
    public void ActivateHitbox(Transform optionalTarget = null)
    {
        //TODO targeting
        // needs target...
        if (hitBoxCollider != null)
            hitBoxCollider.enabled = true;
    }

    public void DeactivateHitbox()
    {
        if (hitBoxCollider != null)
            hitBoxCollider.enabled = false;
    }
    #endregion
}