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


using TMPro;
using UnityEngine;

public enum NarratorExpression
{
    Default,
    Happy,
    Smirk
}
public class NarratorUI : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Animator narratorAnimator;

    #endregion


    #region Internal
    private void Awake()
    {
        panel.SetActive(false);
    }
    #endregion


    #region Methods
    public void DisplayMessage(string text, NarratorExpression state)
    {
        panel.SetActive(true);
        bodyText.text = text;
        if (narratorAnimator != null)
        {
            switch (state)
            {
                case NarratorExpression.Happy:
                    narratorAnimator.SetTrigger("Happy");
                    break;
                case NarratorExpression.Smirk:
                    narratorAnimator.SetTrigger("Smirk");
                    break;
                default:
                    narratorAnimator.SetTrigger("Default");
                    break;
            }
        }
    }
    public void HidePanel()
    {
        panel.SetActive(false);
    }
    #endregion
}