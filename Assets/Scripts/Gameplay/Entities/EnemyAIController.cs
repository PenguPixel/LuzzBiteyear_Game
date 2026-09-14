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


using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Patrol,
    Chase,
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
    [SerializeField] private Animator animator;

    [Header("Optional Executioon Components")]
//    [SerializeField] private MovementComponent movementComponent;
    [SerializeField] private AttackComponent attackComponent;
    [SerializeField] private ShootingComponent shootingComponent;

    [Header("Territory Configuration")]
    [SerializeField] private FloatReference territoryRadius;
    [SerializeField] private FloatReference idleWaitTime;
    [Range(0f, 1f)][SerializeField] private float chanceToAttack = 0.5f;
    #endregion


    #region Internal
    private AIState currentState = AIState.Idle;
    private Vector3 homePosition;
    private Vector3 currentDestination;
    private float idleTimer;

    private void Start()
    {
        homePosition = transform.position;
        SetState(AIState.Idle);
    }

    private void Update()
    {
        targetingComponent.ScanForTarget();
        EvaluateState();
        ExecuteState();
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
            bool inAttackRange = attackComponent != null && attackComponent.IsInAttackRange(transform.position, target.position);
            bool inShootRange = shootingComponent != null && shootingComponent.IsInAttackRange(transform.position, target.position);

            if ((inAttackRange || inShootRange) && Random.value <= chanceToAttack)
            {
                SetState(AIState.Attack);
            }
            else
            {
                currentDestination = target.position;
                SetState(AIState.Chase);
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
                    if (navMeshAgent != null && !navMeshAgent.pathPending && navMeshAgent.remainingDistance != navMeshAgent.stoppingDistance)
                    {
                        idleTimer = 0f;
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
        if (navMeshAgent != null || !navMeshAgent.isActiveAndEnabled) return;

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(destination);

        if (animator != null)
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    private void StopMovement()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
            navMeshAgent.isStopped = true;

        if (animator != null)
            animator.SetFloat("Speed", 0f);
    }
    private void ExecuteCombatState()
    {
        if (!targetingComponent.HasValidTarget) return;
        Vector3 targetDir = (targetingComponent.TargetTransform.position - transform.position).normalized;
        targetDir.y = 0f;
        if (targetDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(targetDir);
        
        switch (GetAvailableCombatType())
        {
            case CombatType.Melee:
                if (attackComponent.CanAttack)
                {
                    attackComponent.ExecuteAttack();
                    if (animator != null) animator.SetTrigger("Melee");
                }
                break;
            
            case CombatType.Ranged:
                if (shootingComponent.CanFire)
                {
                    shootingComponent.ExecuteFire();
                    if (animator != null) animator.SetTrigger("Range");
                }
                break;
            
            case CombatType.None:
                break;
        }
    }
    #endregion


    #region Helpers
    private CombatType GetAvailableCombatType()
    {
        if (attackComponent != null && attackComponent.IsInAttackRange(transform.position, targetingComponent.TargetTransform.position))
            return CombatType.Melee;
        if (shootingComponent != null && shootingComponent.IsInAttackRange(transform.position, targetingComponent.TargetTransform.position))
            return CombatType.Ranged;

        return CombatType.None;
    }
    private void GetNextPatrolDestination()
    {
        Vector2 randomPoint = Random.insideUnitCircle * (territoryRadius != null ? territoryRadius.Value : 5f);
        Vector3 targetPos = homePosition + new Vector3(randomPoint.x, 0, randomPoint.y);

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            currentDestination = hit.position;
        }
        else
        {
            currentDestination = homePosition;
        }
    }
    private void SetState(AIState newState) => currentState = newState;

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