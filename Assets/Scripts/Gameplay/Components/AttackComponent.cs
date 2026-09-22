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
/// This class handles the attack/melee Action of an entity. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Receives the call to attack from its controller and handles attacking.
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: Component on any entity that is expected to attack.
/// </summary>
#endregion


using System.Collections;
using UnityEngine;
[RequireComponent(typeof(TargetingComponent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterAnimationBridge))]
[AddComponentMenu("Combat/Attack Component")]
public class AttackComponent : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif

    [Header("Dependencies")]
    [SerializeField] private TargetingComponent targetingComponent;
    [SerializeField] private MeleeAttack attackPayLoad;
    [SerializeField] private CharacterAnimationBridge animationBridge;

    [Header("Attack Configuration")]
    [SerializeField] private FloatReference attackRange;
    [SerializeField] private FloatReference cooldownTime;
    [SerializeField] private float activeHitboxDuration = 0.3f;

    [Header("Events Configuration")]
    [SerializeField] private GameEvent onAttackExecuted; // maybe redundant

    [Header("Audio")]
    [SerializeField] private AudioEventChannel audioChannel;
    [SerializeField] private SoundData attackSO;
    
    #endregion


    #region Internal
    private float LastTimeAttack;
    private Coroutine activeAttackRoutine;
    private void OnEnable()
    {
        animationBridge.OnMeleeHitFrame += HandleMeleeHitFrame;
        animationBridge.OnAttackComplete += EndAttack;
    }
    private void OnDisable()
    {
        animationBridge.OnMeleeHitFrame -= HandleMeleeHitFrame;
        animationBridge.OnAttackComplete -= EndAttack;    
    }
    #endregion


    #region Public Getters
    public float AttackRange => attackRange != null ? attackRange.Value : 1.5f;
    public bool IsInAttackRange(Vector3 origin, Vector3 targetPos) => Vector3.Distance(origin, targetPos) <= AttackRange;
    public bool CanAttack => Time.time >= LastTimeAttack + (cooldownTime != null ? cooldownTime.Value : 1f);
    #endregion

    
    #region Methods
    public void ExecuteAttack()
    {
        if (!CanAttack) return;
        LastTimeAttack = Time.time;
        if (animationBridge != null)
            animationBridge.TriggerAttack();
        if (onAttackExecuted != null) onAttackExecuted.Raise();
        if (audioChannel != null && attackSO != null)
            audioChannel.RaiseSFX(attackSO, transform.position);
    }
    public void HandleMeleeHitFrame()
    {
        Debug.Log("<color=green>[AttackComponent] AE_OnMeleeHitFrame Received!</color>");
        if (attackPayLoad == null) return;
        
        if (activeAttackRoutine != null) StopCoroutine(activeAttackRoutine);
        activeAttackRoutine = StartCoroutine(DirectAttackRoutine());
    }
    private IEnumerator DirectAttackRoutine()
    {
        Transform target = targetingComponent != null && targetingComponent.HasValidTarget
            ? targetingComponent.TargetTransform : null;
        
        attackPayLoad.ActivateHitbox(gameObject, target, targetingComponent.TargetLayerMask);
        yield return new WaitForSeconds(activeHitboxDuration);
        attackPayLoad.DeactivateHitbox();
        activeAttackRoutine = null;
    }
    public void EndAttack()
    {
        if (activeAttackRoutine != null)
        {
            StopCoroutine(activeAttackRoutine);
            activeAttackRoutine = null;
        }
        if (attackPayLoad != null)
        {
            attackPayLoad.DeactivateHitbox();
        }
    }
    #endregion
}