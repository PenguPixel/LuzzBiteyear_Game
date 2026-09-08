#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-08
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
/// This class handles the targeting for entities. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: [Describe what this class does].
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: [Where it should live in the Scene, Project, Assets'].
/// </summary>
#endregion


using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Combat/Targeting Component")]
public class TargetingComponent : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Detection Setup")]
    [SerializeField] protected FloatReference detectionRange;
    [SerializeField] protected LayerMask targetLayerMask;

    [Header("Output Data")]
    [SerializeField] protected GameEvent onTargetAquired;
    [SerializeField] protected GameEvent onTargetLost;
    #endregion


    #region Internal
    public ITargetable CurrentTarget { get; private set; }
    public Transform TargetTransform => CurrentTarget.TargetTransform;
    public bool HasValidTarget => CurrentTarget != null && CurrentTarget.IsTargetable;
    #endregion

    
    #region Methods
    public void ScanForTarget()
    {
        ITargetable bestTarget = null;
        float closestDistance = detectionRange.Value;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange.Value, targetLayerMask);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            if (hit.TryGetComponent<ITargetable>(out var target) && target.IsTargetable)
            {
                float distance = Vector3.Distance(transform.position, target.TargetTransform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = target;
                }
            }
        }
        
    }
    protected void SetTarget(ITargetable newTarget)
    {
        if (CurrentTarget == newTarget) return;
        CurrentTarget = newTarget;
        if (CurrentTarget != null)
        {
            if (onTargetAquired != null) onTargetAquired.Raise();
        }
        else
        {
            if (onTargetLost != null) onTargetLost.Raise();
        }
    }
    public void ClearTarget() => SetTarget(null);
    #endregion
}


[AddComponentMenu("Combat/Player Targeting Conponent")]
public class PlayerTargeting : TargetingComponent
{
    #region Internal
    private List<ITargetable> availableTargets = new();
    private int currentTargetIndex = -1;
    #endregion


    #region Methods
    public void CycleNextTarget()
    {
        RefreshTargets();
        if (availableTargets.Count == 0)
        {
            ClearTarget();
            return;
        }

        currentTargetIndex = (currentTargetIndex + 1) % availableTargets.Count;
        SetTarget(availableTargets[currentTargetIndex]);
    }
    public void CyclePreviousTarget()
    {
        RefreshTargets();
        if (availableTargets.Count == 0)
        {
            ClearTarget();
            return;
        }
        currentTargetIndex--;
        if (currentTargetIndex < 0) currentTargetIndex = availableTargets.Count - 1;
        SetTarget(availableTargets[currentTargetIndex]);
    }
    private void RefreshTargets()
    {
        availableTargets.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange.Value, targetLayerMask);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            if (hit.TryGetComponent<ITargetable>(out var target) && target.IsTargetable)
            {
                availableTargets.Add(target);
            }
        }

        if (CurrentTarget != null && !availableTargets.Contains(CurrentTarget))
        {
            currentTargetIndex = -1;
        }
    }
    #endregion
}