#region Project Details
/*
* Project: LuzzBiteyear
* Author:Philipp Locher / pengupixels.de
* Issue: Link: https://github.com/PenguPixel/LuzzBiteyear_Game/issues/
* Date: 2026-09-11
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


public class PlayerComponent : MonoBehaviour, ITargetable
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] private PlayerTargeting playerTargeting;
    
    #endregion
    #region Internal
    // private int someValue;

    public Transform TargetTransform => transform;

    public bool IsTargetable => gameObject.activeInHierarchy;
    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (playerTargeting == null)
        {
            playerTargeting = GetComponent<PlayerTargeting>();
        }
    }

    public void Update()
    {
        HandleTargetingState();
    }

    #endregion

    #region Methods
    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    private void HandleTargetingState()
    {
        if (playerTargeting == null) return;

        if (playerTargeting.HasValidTarget)
        {
            float currentDistance = Vector3.Distance(transform.position, playerTargeting.TargetTransform.position);
            float maxRange = 15f; // Example max range, implement value from SO later;

            if (currentDistance > maxRange)
            {
                playerTargeting.ClearTarget();
            }
        }
        else
        {
            playerTargeting.ScanForTarget();
        }
    }

    #endregion
}