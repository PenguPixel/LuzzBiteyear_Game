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


using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("Dependencies")]
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup panelCanvasGrp;
    [SerializeField] private GameObject faceImage;
    [SerializeField] private TextMeshProUGUI bodyText;
    
    [Header("Faces")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite happySprite;
    [SerializeField] private Sprite smirkSprite;

    [Header("Timer")]
    [SerializeField] private float panelFadeDuration = 0.3f;
    [SerializeField] private float messageDisplayTime = 3.0f;
    #endregion


    #region Internal
    private Image image;
    private void Awake()
    {
        panel.SetActive(false);
        image = faceImage.GetComponent<Image>();
    }
    #endregion


    #region Methods
    public void DisplayMessage(string text, NarratorExpression state)
    {
        bodyText.text = text;
        switch (state)
        {
            case NarratorExpression.Happy:
                image.sprite = happySprite;
                break;
            case NarratorExpression.Smirk:
                image.sprite = smirkSprite;
                break;
            default:
                image.sprite = defaultSprite;
                break;
        }
        StartCoroutine(NarratorFadeIn(panelFadeDuration));
    }
    public void HidePanel()
    {
        StartCoroutine(NarratorFadeOut(panelFadeDuration));
        // panel.SetActive(false);
    }
    #endregion

    #region Coroutines
    private IEnumerator NarratorFadeIn(float duration = 0.8f)
    {
        panelCanvasGrp.alpha = 0f;
        panel.SetActive(true);
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            panelCanvasGrp.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }
        panelCanvasGrp.alpha = 1f;
    } 

    private IEnumerator NarratorFadeOut(float duration = 0.8f)
    {
        if (panelCanvasGrp == null) yield break;

        panelCanvasGrp.alpha = 1f;
        panel.SetActive(true);

        if (messageDisplayTime > 0)
        {
            yield return new WaitForSecondsRealtime(messageDisplayTime);
        }

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            panelCanvasGrp.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            yield return null;
        }
        panelCanvasGrp.alpha = 0f;
        panel.SetActive(false);
    }
    #endregion
}