#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-28
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


[RequireComponent(typeof(CanvasGroup))]
public class WorldInteractionUI : MonoBehaviour
{
    #region Inspector
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI interactionTextDisplay;

    private Camera mainCamera;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        mainCamera = Camera.main;
        Hide();
    }

    private void LateUpdate()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        transform.rotation = mainCamera.transform.rotation;
    }

    public void Show(string interactionText)
    {
        gameObject.SetActive(true);

        if (interactionText != null) interactionTextDisplay.text = interactionText;

        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
    public void Hide()
    {
        canvasGroup.alpha = 0f;
    }
    #endregion


}