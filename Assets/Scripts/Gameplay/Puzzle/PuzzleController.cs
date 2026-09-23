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


public class PuzzleController : MonoBehaviour
{
    #region Singleton
    public static PuzzleController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif

    [Header("Dependencies")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private List<PuzzleNode> nodes = new List<PuzzleNode>();    
    #endregion

    #region Internal
    private PlugTerminal _activeTerminal;
    #endregion    
    
    #region Methods
    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    public void SetupCurrentPuzzle(PlugTerminal terminal)
    {
        _activeTerminal = terminal;

        foreach (var node in nodes)
        {
            node.Scramble();
        }
    }

    public void CheckState()
    {
        foreach (var node in nodes)
        {
            if (!node.IsConnected) return;
        }

        CompletePuzzle();
    }

    private void CompletePuzzle()
    {
        if (_activeTerminal != null)
        {
            _activeTerminal.NotifySolved();
        }
        if (gameManager != null)
        {
            gameManager.ExitPuzzle();
        }
    }

    public void AbortPuzzle()
    {
        if (gameManager != null)
        {
            gameManager.ExitPuzzle();
        }
    }
    #endregion
}