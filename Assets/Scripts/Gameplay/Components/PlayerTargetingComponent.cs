#region Project Details
/*
* Project: LuzzBiteyear
* Author:Philipp Locher / pengupixels.de
* Issue: Link: https://github.com/PenguPixel/LuzzBiteyear_Game/issues/
* Date: 2026-09-14
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
using System.Collections.Generic;
using UnityEngine.UI;

[AddComponentMenu("Combat/Player Targeting Component")]
public class PlayerTargeting : TargetingComponent
{
    [Header("Visual Feedback")]
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] Image cursorImage;
    [SerializeField] private float worldOffset;

    #region Internal
    private readonly List<ITargetable> _availableTargets = new();
    private int _currentTargetIndex = -1;
    private void LateUpdate()
    {
        if (!HasValidTarget || worldCanvas == null) return;

        worldCanvas.transform.position = new Vector3(CurrentTarget.TargetTransform.position.x, CurrentTarget.TargetTransform.position.y + worldOffset, CurrentTarget.TargetTransform.position.z);

        if (Camera.main != null)
        {
            worldCanvas.transform.rotation = Camera.main.transform.rotation;
        }    
    }
    #endregion


    #region Methods
    public void CycleNextTarget()
    {
        RefreshTargets();
        if (_availableTargets.Count == 0)
        {
            ClearTarget();
            return;
        }

        _currentTargetIndex = (_currentTargetIndex + 1) % _availableTargets.Count;
        SetTarget(_availableTargets[_currentTargetIndex]);
    }
    public void CyclePreviousTarget()
    {
        RefreshTargets();
        if (_availableTargets.Count == 0)
        {
            ClearTarget();
            return;
        }
        _currentTargetIndex--;
        if (_currentTargetIndex < 0) _currentTargetIndex = _availableTargets.Count - 1;
        SetTarget(_availableTargets[_currentTargetIndex]);
    }
    private void RefreshTargets()
    {
        _availableTargets.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange.Value, targetLayerMask);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            if (hit.TryGetComponent<ITargetable>(out var target) && target.IsTargetable)
            {
                _availableTargets.Add(target);
            }
        }

        if (CurrentTarget != null && !_availableTargets.Contains(CurrentTarget))
        {
            _currentTargetIndex = -1;
        }
    }
    #endregion
}