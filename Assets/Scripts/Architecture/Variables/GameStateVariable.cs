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
/// This class handles GameState communication. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: It holds an enum to define valid game states. It handles the communication in our EventSystem for other components.
/// Coordination: The GameManager decides the current GameState and sets the value on demand. Then all Listeners will be informed on the change as per usual with any other Variable.
/// Deployment: GameStates are defined once in the folder ScriptableObjects/GameStates.
/// </summary>
#endregion


using UnityEngine;

public enum GameState
{
    Exploration,
    Combat,
    Paused,
    Puzzle,
    GameOver
}

[CreateAssetMenu(menuName = "Variables/GameState")]
public class GameStateVariable : ScriptableObject
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] private GameState value ;
    
    #endregion
    #region Internal
    public GameState Value => value;
    #endregion

    
    #region Methods
    public void SetValue(GameState newValue)
    {
        value = newValue;
    }
    #endregion
}