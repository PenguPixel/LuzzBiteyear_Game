#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-10
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
/// This class handles high level decision making and state execution for enemy entities.
/// It delegates combat actions to AttackComponent or Shooting Compnent and delegates movement to NavMeshAgent.
/// </para>
/// </remarks>
/// <summary>
/// Description: Decides entity actions based on territory bounds and targeting state.
/// Coordination: Queries TargetingComponent, BaseEntity, AttackComponent, ShootingComponent, and NavMeshAgent.
/// Deployment: Attached to Enemy Entity Prefabs.
/// </summary>
#endregion


using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Patrol,
    Chase,
    Reposition,
    Attack,
}

public enum CombatType
{
    None,
    Melee,
    Ranged
}

[RequireComponent(typeof(TargetingComponent))]
[RequireComponent(typeof(EntityBase))]
[AddComponentMenu("Combat/AIController")]
public class EnemyAIController : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Required Components")]
    [SerializeField] private TargetingComponent targetingComponent;
    [SerializeField] private EntityBase entityBase;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private CharacterAnimationBridge animationBridge;

    [Header("Optional Executioon Components")]
//    [SerializeField] private MovementComponent movementComponent;
    [SerializeField] private AttackComponent attackComponent;
    [SerializeField] private ShootingComponent shootingComponent;

    [Header("Pacing/Timing")]
    [SerializeField] private float decisionInterval = 0.2f;
    [SerializeField] private float attackRecoveryTime = 0.5f;

    [Header("Territory Configuration")]
    [SerializeField] private FloatReference territoryRadius;
    [SerializeField] private FloatReference idleWaitTime;
    [Range(0f, 1f)][SerializeField] private float chanceToAttack = 0.5f;
    [Range(0.1f, 0.7f)][SerializeField] private float minRangeRatio = 0.3f;

    [Header("Combat Weighting")]
    [Range(0f, 100f)][SerializeField] private float meleeWeight = 50f;
    [Range(0f, 100f)][SerializeField] private float rangedWeight = 50f;
    #endregion


    #region Internal
    private AIState currentState = AIState.Idle;
    private Vector3 homePosition;
    private Vector3 currentDestination;
    private float idleTimer;
    private float decisionTimer;
    private bool isPerformingAction;
    private Coroutine recoveryRoutine;

    private void Start()
    {
        homePosition = transform.position;

        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
            navMeshAgent.Warp(transform.position);
        
        SetState(AIState.Idle);
    }
    private void OnEnable()
    {
        if (animationBridge != null)
        {
            animationBridge.OnAttackComplete += HandleActionCompleted;
            animationBridge.OnHitReactionComplete += HandleActionCompleted;
        }
    }
    private void OnDisable()
    {
        if (animationBridge != null)
        {
            animationBridge.OnAttackComplete -= HandleActionCompleted;
            animationBridge.OnHitReactionComplete -= HandleActionCompleted;
        }
    }
    private void Update()
    {
        targetingComponent.ScanForTarget();
        if (isPerformingAction)
        {
            if (currentState != AIState.Attack)
                ExecuteState();
            if (animationBridge != null)
                UpdateLocomotiveVisual();
            return;
        }
        decisionTimer += Time.deltaTime;
        if (decisionTimer >= decisionInterval)
        {
            decisionTimer = 0f;
            EvaluateState();    
        }
        ExecuteState();
        if (animationBridge != null)
            UpdateLocomotiveVisual();
    }

    #endregion


    #region Decision Logic
    private void EvaluateState()
    {
        if (targetingComponent.HasValidTarget)
        {
            Transform target = targetingComponent.TargetTransform;
            float distanceToHome = Vector3.Distance(target.position, homePosition);
            // Check distance to player against boundaries
            if (distanceToHome > territoryRadius.Value)
            {
                targetingComponent.ClearTarget();
                SetState(AIState.Idle);
                return;
            }

            //Query components to ready actions
            CombatType availableCombat = GetAvailableCombatType();
            bool canExecuteCombat = (availableCombat == CombatType.Melee && attackComponent != null && attackComponent.CanAttack)
                                    || (availableCombat == CombatType.Ranged && shootingComponent != null && shootingComponent.CanFire);

            if (canExecuteCombat && Random.value <= chanceToAttack)
            {
                SetState(AIState.Attack);
                return;
            }
            else
            {
                bool isInMelee = attackComponent != null && attackComponent.IsInAttackRange(transform.position, target.position);
                bool prefersRanged = rangedWeight > meleeWeight;
                if (shootingComponent != null && isInMelee && prefersRanged)
                {
                    CalculateRepositionDesination(target.position);
                    SetState(AIState.Reposition);
                }
                else
                {
                    float offset = attackComponent != null ? attackComponent.AttackRange * 0.2f : 1f;
                    Vector3 dirPlayer = (transform.position - target.position).normalized;
                    currentDestination = target.position + (dirPlayer * offset);
                    SetState(AIState.Chase);
                }
            }
        }
        else
        {
            //Patrol Loop
            switch (currentState)
            {
                case AIState.Idle:
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= idleWaitTime.Value)
                    {
                        GetNextPatrolDestination();
                        SetState(AIState.Patrol);
                    }
                    break;

                case AIState.Patrol:
                    if (navMeshAgent != null && !navMeshAgent.pathPending && navMeshAgent.hasPath && (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.1f))
                    {
                        SetState(AIState.Idle);
                    }
                    break;
                
                default:
                    SetState(AIState.Idle);
                    break;
            }
        }
    }

    #endregion


    #region Execution Logic
    private void ExecuteState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                StopMovement();
                break;
            
            case AIState.Patrol:
            case AIState.Chase:
            case AIState.Reposition:
                MoveToDestination(currentDestination);
                break;

            case AIState.Attack:
                StopMovement();
                ExecuteCombatState();
                break;
        }
    }

    private void MoveToDestination(Vector3 destination)
    {
        if (navMeshAgent == null || !navMeshAgent.isActiveAndEnabled) return;

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(destination);
    }

    private void StopMovement()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
        }

        if (animationBridge != null)
            animationBridge.UpdateLocomotion(0f);
    }
    private void ExecuteCombatState()
    {
        if (!targetingComponent.HasValidTarget)
        {
            SetState(AIState.Chase);
            return;
        } 
        Vector3 targetDir = (targetingComponent.TargetTransform.position - transform.position).normalized;
        targetDir.y = 0f;
        if (targetDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(targetDir);
        bool actionStarted = false;
        switch (GetAvailableCombatType())
        {
            case CombatType.Melee:
                if (attackComponent != null && attackComponent.CanAttack)
                {
                    actionStarted = true;
                    attackComponent.ExecuteAttack();
                }
                break;
            
            case CombatType.Ranged:
                if (shootingComponent != null && shootingComponent.CanFire)
                {
                    actionStarted = true;
                    shootingComponent.ExecuteFire();
                }
                break;
            
            case CombatType.None:
                break;
        }
        if (actionStarted)
        {
            isPerformingAction = true;
        }
        else
        {
            SetState(AIState.Chase);
        }
    }
    private void UpdateLocomotiveVisual()
    {
        if (animationBridge != null && navMeshAgent != null)
        {
            float speed = navMeshAgent.isStopped ? 0f : navMeshAgent.velocity.magnitude;
            animationBridge.UpdateLocomotion(speed);
        }
    }
    private void HandleActionCompleted()
    {
        if (recoveryRoutine != null) StopCoroutine(recoveryRoutine);
        recoveryRoutine = StartCoroutine(RecoveryRoutine());
        Debug.Log("Action Completed Event Fired");
    }
    private IEnumerator RecoveryRoutine()
    {
        yield return new WaitForSeconds(attackRecoveryTime);
        isPerformingAction = false;
        SetState(AIState.Chase);
    }
    #endregion


    #region Helpers
    private CombatType GetAvailableCombatType()
    {
        if (!targetingComponent.HasValidTarget) return CombatType.None;
        bool canMelee = attackComponent != null && attackComponent.IsInAttackRange(transform.position, targetingComponent.TargetTransform.position);
        bool canShoot = shootingComponent != null && shootingComponent.IsInAttackRange(transform.position, targetingComponent.TargetTransform.position);
        if (canMelee && canShoot)
        {
            float totalWeight = meleeWeight + rangedWeight;
            if (totalWeight <= 0f) return CombatType.Melee;

            float roll = Random.Range(0f, totalWeight);
            return (roll < meleeWeight) ? CombatType.Melee : CombatType.Ranged;
        }
        if (canMelee) return CombatType.Melee;
        if (canShoot) return CombatType.Ranged;

        return CombatType.None;
    }
    private void GetNextPatrolDestination()
    {
        Vector2 randomPoint = Random.insideUnitCircle * (territoryRadius.Value >= 0.5f ? territoryRadius.Value : 5f);
        Vector3 targetPos = homePosition + new Vector3(randomPoint.x, 0, randomPoint.y);

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
        {
            currentDestination = hit.position;
        }
        else
        {
            currentDestination = homePosition + new Vector3(randomPoint.x, 0, randomPoint.y);
        }
    }
    private void CalculateRepositionDesination(Vector3 targetPos)
    {
        if (navMeshAgent != null && navMeshAgent.hasPath && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance + 0.5f) 
            return;

        float maxRange = shootingComponent.AttackRange;
        float minRange = maxRange * minRangeRatio;
        float currentDistance = Vector3.Distance(transform.position, targetPos);
        Vector3 dirFromTarget = (transform.position - targetPos).normalized;

        Vector3 desiredPos;
        if (currentDistance < minRange)
        {
            // Backs away from the traget
            desiredPos = targetPos + (dirFromTarget * minRange);
        }
        else
        {
            // Picks a random direction to strafe to
            Vector3 strafeDir = Vector3.Cross(dirFromTarget, Vector3.up);
            if (Random.value > 0.5f) strafeDir = -strafeDir;

            desiredPos = transform.position + (strafeDir * 3f);
        }

        // Safety GuardRail to keep it in territory
        Vector3 offsetFromHome = desiredPos - homePosition;
        if (offsetFromHome.magnitude > territoryRadius.Value)
        {
            desiredPos = homePosition + (offsetFromHome.normalized * territoryRadius.Value);
        }
        // Validation on NavMesh
        if (NavMesh.SamplePosition(desiredPos, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
        {
            currentDestination = hit.position;
        }
        else
        {
            currentDestination = transform.position;
        }

    }
    private void SetState(AIState newState)
    {
        if (currentState != AIState.Idle && newState == AIState.Idle)
            idleTimer = 0f;
        currentState = newState;
    } 

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? homePosition : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(center, territoryRadius != null ? territoryRadius.Value : 5f);      
    }
#endif
    #endregion
}