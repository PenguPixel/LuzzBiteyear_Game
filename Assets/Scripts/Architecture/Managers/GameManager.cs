#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-07
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
/// This class handles state of the game and enforces rules. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Tracks the state of the game. Communicates to other components if needed.
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: Sits as a global object on start.
/// </summary>
#endregion


using UnityEngine;


public class GameManager : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("State Data")]
    [SerializeField] private GameStateVariable currentGameState;

    [Header("Broadcasting Events")]
    [SerializeField] private GameEvent onGameStateChanged;
    [SerializeField] private GameEvent onGamePaused;
    [SerializeField] private GameEvent onGameResumed;
    
    #endregion
    #region Internal
    private GameState previousState;

    private void Start()
    {
        
    }
    #endregion


    #region Methods
    /// <summary>
    /// Sets the current game state. Raises the broadcasting Event.
    /// </summary>
    /// <param name = "GameState"> Reference to a validated Game State</param>
    public void SetGameState(GameState newState)
    {
        if (currentGameState == null) return;
        if (currentGameState.Value == newState) return;

        previousState = currentGameState.Value;
        currentGameState.SetValue(newState);

        if(onGameStateChanged != null) onGameStateChanged.Raise();
    }

    public void TogglePause()
    {
        if (currentGameState.Value == GameState.Paused)
        {
            SetGameState(previousState);
            Time.timeScale = 1f;
            onGameResumed?.Raise();
        }
        else
        {
            SetGameState(GameState.Paused);
            Time.timeScale = 0f;
            onGamePaused?.Raise();
        }
    }
    #endregion
}