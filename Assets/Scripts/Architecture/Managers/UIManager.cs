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


using System.Collections;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Game State")]
    [SerializeField] GameStateVariable currentGameState ;

    [Header("UIPanels")]
    [SerializeField] private GameObject HUDPanel;
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject PuzzlePanel;
    [Header("Start Screen")]
    [SerializeField] private GameObject StartScenePanel;
    [SerializeField] private CanvasGroup blackFaderCanvasGroup;
    [SerializeField] private GameObject readyPrompt;
    #endregion


    #region Internal
    private void HideAllPanels()
    {
        if (MenuPanel != null) MenuPanel.SetActive(false);
        if (HUDPanel != null) HUDPanel.SetActive(false);
        if (GameOverPanel != null) GameOverPanel.SetActive(false);
        if (PuzzlePanel != null) PuzzlePanel.SetActive(false);
        if (StartScenePanel != null) StartScenePanel.SetActive(false);
    }
    #endregion

    
    #region Methods
    public void HandleGameStateChanged()
    {
        if (currentGameState == null) return;

        HideAllPanels();


        switch (currentGameState.Value)
        {
            case GameState.Exploration:
                if (HUDPanel != null) HUDPanel.SetActive(true);
                break;
            case GameState.Combat:
                if (HUDPanel != null) HUDPanel.SetActive(true);
                break;
            
            case GameState.Paused:
                if (MenuPanel != null) MenuPanel.SetActive(true);
                break;

            case GameState.GameOver:
                if (GameOverPanel != null) GameOverPanel.SetActive(true);
                break;
            
            case GameState.Puzzle:
                if (PuzzlePanel != null) PuzzlePanel.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;

            case GameState.StartScene:
                if (StartScenePanel != null) StartScenePanel.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    public void ShowReadyPrompt(bool show)
    {
        if (readyPrompt != null) readyPrompt.SetActive(true);
    }
    #endregion

    #region Coroutines
    public IEnumerator FadeInFromBlack(float duration = 1.0f)
    {
        if (blackFaderCanvasGroup == null) yield break;
        blackFaderCanvasGroup.gameObject.SetActive(true);
        blackFaderCanvasGroup.alpha = 1f;
        blackFaderCanvasGroup.blocksRaycasts = true;

        yield return null;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            blackFaderCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            yield return null;
        }
        blackFaderCanvasGroup.alpha = 0f;
        blackFaderCanvasGroup.blocksRaycasts = false;
    }

    public IEnumerator FadeToBlack(float duration = 0.8f)
    {
        if (blackFaderCanvasGroup == null) yield break;

        blackFaderCanvasGroup.gameObject.SetActive(true);
        blackFaderCanvasGroup.blocksRaycasts = true;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            blackFaderCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }
        blackFaderCanvasGroup.alpha = 1f;
    }
    #endregion
}