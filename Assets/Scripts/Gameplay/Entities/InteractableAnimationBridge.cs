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
/// Bridges door logic components with the Unity Animator. Caches parameter hashes and routes animation events.
/// </para>
/// </remarks>
/// <summary>
/// Description: Triggers interactable animator parameters and translates animation keyframe events into C# actions.
/// Coordination: Directly attached to the interactable visual/animator root, consumed by wired components.
/// Deployment: Placed on the interactable GameObject alongside Animator.
/// </summary>
#endregion

using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class InteractableAnimationBridge : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] private Animator animator;
    #endregion
    
    #region Actions & Events
    // Door
    public event Action OnDoorOpenComplete;
    #endregion

    #region Internal
    private static readonly int IsOpenHash = Animator.StringToHash("IsOpen");
    private static readonly int OpenTriggerHash = Animator.StringToHash("OpenTrigger");
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    #endregion

    #region Methods    
    public void SetOpenState(bool isOpen) => animator.SetBool(IsOpenHash, isOpen);
    public void TriggerOpen() => animator.SetTrigger(OpenTriggerHash);
    #endregion

    #region Animation Events
    public void AE_OnDoorOpenComplete() => OnDoorOpenComplete?.Invoke();
    #endregion
}