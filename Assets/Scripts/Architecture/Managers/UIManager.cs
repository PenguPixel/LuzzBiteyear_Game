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
/// This class handles display management of overarching UI Elements. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Glorified light switch operator.
/// Coordination: whenever the gamestate changes or the player is navigating the Menu, it switches lights on and off.
/// Deployment: static object.
/// </summary>
#endregion


using UnityEngine;


public class UIManager : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] GameStateVariable currentGameState ;

    [Header("UIPanels")]
    [SerializeField] private GameObject HUDPanel;
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject GameOverPanel;
    #endregion


    #region Internal
    private void HideAllPanels()
    {
        if (MenuPanel != null) MenuPanel.SetActive(false);
        if (HUDPanel != null) HUDPanel.SetActive(false);
        if (GameOverPanel != null) GameOverPanel.SetActive(false);
    }
    #endregion

    
    #region Methods
    public void HandleGameStateChanged()
    {
        if (currentGameState == null) return;


        switch (currentGameState.Value)
        {
            case GameState.Exploration:
            case GameState.Combat:
                if (HUDPanel != null) HUDPanel.SetActive(true);
                break;
            
            case GameState.Paused:
                if (MenuPanel != null) MenuPanel.SetActive(true);
                break;

            case GameState.GameOver:
                if (GameOverPanel != null) GameOverPanel.SetActive(true);
                break;
        }

    }
    #endregion
}