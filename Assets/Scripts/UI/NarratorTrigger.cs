#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-21
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

[RequireComponent(typeof(Collider))]
public class NarratorTrigger : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [TextArea(3,6)]
    [SerializeField] private string narratorText ;
    [SerializeField] private NarratorExpression narratorExpression;

    [SerializeField] private bool hideOnExit = true;
    [SerializeField] private bool isOneShot = false;
    #endregion


    #region Internal
    private bool hasTriggered = false;
    #endregion


    #region Methods
    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && isOneShot) return;
        if (other.CompareTag("Player"))
        {
            NarratorUI narratorUI = FindFirstObjectByType<NarratorUI>();
            if (narratorUI != null)
            {
                narratorUI.DisplayMessage(narratorText, narratorExpression);
                hasTriggered = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!hideOnExit) return;
        if (other.CompareTag("Player"))
        {
            NarratorUI narratorUI = FindFirstObjectByType<NarratorUI>();
            narratorUI.HidePanel();
        }
    }
    #endregion
}