#region Project Details
/*
* Project: LuzzBiteyear
* Author:Philipp Locher / pengupixels.de
* Issue: Link: https://github.com/PenguPixel/LuzzBiteyear_Game/issues/
* Date: 2026-09-15
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
/// Represents an in-world socket for Luzz's tail plug.
/// Implements IInteractable to trigger the puzzle overlay via GameManager.
/// </para>
/// </remarks>
/// <summary>
/// Description: Initiates GameState.Puzzle when interacted with and executes unlock logic upon puzzle completion.
/// Coordination: Implements IInteractable using InteractionData SO; communicates with GameManager and CircuitPuzzleController.
/// Deployment: Attached to console or socket GameObjects next to barriers/doors.
/// </summary>
#endregion


using UnityEngine;
using UnityEngine.Events;


public class PlugTerminal : MonoBehaviour, IInteractable
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Interaction Config SO")]
    [SerializeField] private InteractionData interactionData;

    [Header("Dependencies")]
    [SerializeField] private GameManager gameManager;

    [Header("Puzzle state and events")]
    [SerializeField] private bool isSolved = false;
    [SerializeField] private UnityEvent onUnlocked;
    
    #endregion

    #region Public Getters / IInteractable
    public InteractionData InteractionData => interactionData;
    public bool IsSolved => isSolved;

    #endregion


    #region Methods
    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    public void Interact(GameObject interactor)
    {
        if (isSolved)
        {
            Debug.Log("[PlugTerminal] is already solved.");
            return;
        }

        if (gameManager == null)
        {
            Debug.Log("[PlugTerminal] Game Manager reference missing.");
            return;
        }

        PuzzleController.Instance.SetupCurrentPuzzle(this);

        gameManager.EnterPuzzle();
    }

    public void NotifySolved()
    {
        isSolved = true;
        onUnlocked?.Invoke();
        Debug.Log("[Plug Terminal] Puzzle solved, target door opened.");
    }
    #endregion
}