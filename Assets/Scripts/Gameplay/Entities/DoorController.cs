#region Project Details
/*
* Project: LuzzBiteyear
* Author: Philipp Locher / pengupixels.de
* Issue: Link: https://github.com/PenguPixel/LuzzBiteyear_Game/issues/
* Date: 2026-09-21
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
/// Controls the animated door/barrier entity. Listens to puzzle completion events and drives state on its Animator./// </para>
/// </remarks>
/// <summary>
/// Description: Receives puzzle resolution signals, triggers door open/close animations via Animator hashes, and manages collision barriers.
/// Coordination: Directly invoked by PlugTerminal via UnityEvents or code; dispatches completion events back to listeners.
/// Deployment: Attached to the root GameObject of an animated door or barrier.
/// </summary>
#endregion

using System;
using UnityEngine;

[RequireComponent(typeof(InteractableAnimationBridge))]
public class DoorController : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif

    [Header("References")]
    [SerializeField] private InteractableAnimationBridge animationBridge;
    [SerializeField] private Collider doorCollider;

    [Header("Door Config")]
    [SerializeField] private bool startsOpen = false;    
    [SerializeField] private bool disableColliderImmediately = true;
    #endregion

    #region Internal
    private bool _isOpen;
    #endregion

    #region Public Getters
    public bool IsOpen => _isOpen;
    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        if (animationBridge == null)
        {
            animationBridge = GetComponent<InteractableAnimationBridge>();
        }

        _isOpen = startsOpen;
        animationBridge.SetOpenState(_isOpen);

        if (doorCollider != null)
        {
            doorCollider.enabled = !_isOpen;
        }
    }

    private void OnEnable()
    {
        if (animationBridge != null)
        {
            animationBridge.OnDoorOpenComplete += HandleDoorOpenComplete;
        }
    }

    private void OnDisable()
    {
        if (animationBridge != null)
        {
            animationBridge.OnDoorOpenComplete -= HandleDoorOpenComplete;
        }
    }
    #endregion

    #region Methods
    public void ReceivePuzzleResult(bool isSolved)
    {
        if (isSolved)
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (_isOpen) return;

        _isOpen = true;
        animationBridge.SetOpenState(true);
        animationBridge.TriggerOpen();

        if (disableColliderImmediately && doorCollider != null)
        {
            doorCollider.enabled = false;
        }
    }
    #endregion

    #region Event Handlers
    private void HandleDoorOpenComplete()
    {
        if (!disableColliderImmediately && doorCollider != null)
        {
            doorCollider.enabled = false;
        }
    }
    #endregion
}