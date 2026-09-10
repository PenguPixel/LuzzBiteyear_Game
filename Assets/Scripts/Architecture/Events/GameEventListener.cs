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
/// This class handles [Core Responsibility]. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: [Describe what this class does].
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: [Where it should live in the Scene, Project, Assets'].
/// </summary>
#endregion


using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Events;


public class GameEventListener : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Event Configuration")]
    [Tooltip("EventSO channel to listen to")]
    [SerializeField] private GameEvent gameEvent;

    [Header("Response")]
    [Tooltip("UnityEvent action to invoke when the event is raised")]
    [SerializeField] private UnityEvent response;
    #endregion
    #region Internal
    #endregion


    #region Methods
    private void OnEnable()
    {
        if (gameEvent != null) gameEvent.SubscribeListener(this);
    }
    private void Oisable()
    {
        if(gameEvent != null) gameEvent.UnsubscribeListener(this);       
    }
    public void OnEventRaised()
    {
        response?.Invoke();
    }
    #endregion
}