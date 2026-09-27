#region Project Details
/*
* Project: Luzz Biteyear
* Author:Philipp Locher / www.pengupixels.de
* Issue: Link: https://github.com/PenguPixel/LuzzBiteyear_Game
* Date: 2026-09-26
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

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class StartSceneManager : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Dependencies")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ScenePreloader preloader;

    [Header("Sequence Settings")]
    [SerializeField] private float fadeDuration = 0.8f;
    #endregion

    #region Internal
    private bool _isLevelReady = false;
    private bool _transitionStarted = false;
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (preloader != null)
        {
            preloader.OnPreloadComplete += HandleLevelready;
        }

        StartCoroutine(WelcomeIntroRoutine());
    }

    private void Update()
    {
        if (_isLevelReady && !_transitionStarted)
        {
            if (WasAnyStartTriggerPressed())
            {
                TriggerGameStart();
            }
        }
    }

    private void OnDestroy()
    {
        if (preloader != null)
        {
            preloader.OnPreloadComplete -= HandleLevelready;
        }
    }
    #endregion

    
    #region Methods
    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    private bool WasAnyStartTriggerPressed()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) return true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame) return true;
        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame ||
               Gamepad.current.buttonEast.wasPressedThisFrame ||
               Gamepad.current.buttonWest.wasPressedThisFrame ||
               Gamepad.current.buttonNorth.wasPressedThisFrame)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    private IEnumerator WelcomeIntroRoutine()
    {
        if (gameManager != null)
        {
            yield return StartCoroutine(uiManager.FadeInFromBlack(fadeDuration));
        }
    }

    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    private void HandleLevelready()
    {
        _isLevelReady = true;

        if (uiManager != null)
        {
            uiManager.ShowReadyPrompt(true);
        }
    }

    public void TriggerGameStart()
    {
        if (_transitionStarted) return;
        _transitionStarted = true;

        StartCoroutine(TransitionToGameplayRoutine());
    }

    private IEnumerator TransitionToGameplayRoutine()
    {
        if (uiManager != null)
        {
            yield return StartCoroutine(uiManager.FadeToBlack(fadeDuration));
        }

        if (preloader != null)
        {
            preloader.ActivateScene();
        }
        else
        {
            Debug.Log("[StartSceneManager] Scene preloader reference missing!");
        }
    }
    #endregion
}