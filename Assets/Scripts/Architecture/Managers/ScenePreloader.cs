#region Project Details
/*
* Project: Luzz Biteyear
* Author:Philipp Locher / www.pengupixels.de
* Issue: Link: https://github.com/PenguPixel/LuzzBiteyear_Game
* Date: 2026-09-27
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


using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ScenePreloader : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Configs")]
    [SerializeField] private string targetSceneName = "Level_00" ;    
    #endregion

    #region Internal
    private AsyncOperation _loadOp;
    #endregion

    #region Public Getters
    public bool IsReady {get; private set;} = false;
    public event Action OnPreloadComplete;
    #endregion

    #region Unity Methods
    private void Start()
    {
        StartCoroutine(PreloadRoutine());
    }
    #endregion
    
    #region Methods
    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    private IEnumerator PreloadRoutine()
    {
        _loadOp = SceneManager.LoadSceneAsync(targetSceneName);
        if (_loadOp == null)
        {
            Debug.Log($"[ScenePreloader] target Scene '{targetSceneName}' not found");
            yield break;
        }

        _loadOp.allowSceneActivation = false;

        while (_loadOp.progress < 0.9f)
        {
            yield return null;
        }

        IsReady = true;
        OnPreloadComplete?.Invoke();
    }

    public void ActivateScene()
    {
        if (_loadOp != null)
        {
            _loadOp.allowSceneActivation = true;
        }
    }
    #endregion
}