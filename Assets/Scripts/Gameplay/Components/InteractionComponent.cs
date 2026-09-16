#region Project Details
/*
* Project: LuzzBiteyear
* Author:Philipp Locher / pengupixels.de
* Issue: Link: https://github.com/PenguPixel/LuzzBiteyear_Game/issues/
* Date: 2026-09-16
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


public class InteractionComponent : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif

    [Header("Detection")]
    [SerializeField] private FloatReference interactionRadius;
    [SerializeField] private LayerMask interactionLayerMask;
    #endregion

    #region Internal
    private IInteractable _currentNearbyInteractable;
    #endregion
    
    #region Public Getters
    public IInteractable CuurentNearbyInteractable => _currentNearbyInteractable;
    public bool HasNearbyInteractable => _currentNearbyInteractable != null;
    #endregion

    #region Unity Methods
    private void Update()
    {
        DetectNearbyInteractable();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    private void DetectNearbyInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius.Value, interactionLayerMask);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IInteractable>(out var interactable))
            {
                _currentNearbyInteractable = interactable;
                // Debug.Log("[InteractionComponent] Interactable in Range acquired!");
                return;
            }
        }

        _currentNearbyInteractable = null;
    }

    public void ExecuteInteraction()
    {
        if (_currentNearbyInteractable != null)
        {
            Debug.Log("[InteractionComponent] Execute Interaction!");
            _currentNearbyInteractable.Interact(gameObject);
        }
        else
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, interactionRadius.Value, interactionLayerMask);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.Interact(gameObject);
                    break;
                }
            }
        }
    }
    #endregion

#if UNITY_EDITOR
    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius.Value);
    }
    #endregion
#endif
}