#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-15
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
/// This class handles UIUpdates to the Resource allocated to this. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Reacts to GameEvents and updates the UI.
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: Append this to the Resource UI and configure it accordingly.
/// </summary>
#endregion


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResourceUI : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Resource Configuration")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private IntReference currentResource;
    [SerializeField] private IntReference maxResource;

    [Header("Visual Feddback Configuration")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color warningColor;
    [SerializeField] private Color criticalColor;

    [Header("Treshhold Configuration")]
    [Range(0f,1f)][SerializeField] private float warningThreshold = 0.5f;
    [Range(0f,1f)][SerializeField] private float criticalThreshold = 0.25f;

    [Header("Blink Settings")]
    [SerializeField] private float blinkDuration = 0.3f;
    [SerializeField] private int blinkCount = 3;
    #endregion


    #region Internal
    private readonly List<Image> fillImages = new();
    private Coroutine activeBlinkRoutine;
    private int previousResourceValue;
    private void Start()
    {
        InitializeBar();
        previousResourceValue = currentResource != null ? currentResource.Value : 0;
        UpdateResoureUI();
    }
    private void OnEnable()
    {
        UpdateResoureUI();
    }
    #endregion


    #region Methods
    public void InitializeBar()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        fillImages.Clear();

        for (int i = 0; i < maxResource.Value; i++)
        {
            GameObject frameInstance = Instantiate(prefab, transform);
            if (frameInstance.transform.GetChild(0).TryGetComponent<Image>(out var fillImage))
            {
                fillImages.Add(fillImage);
            }
        }
    }
    public void UpdateResoureUI()
    {
        float ratio = maxResource.Value > 0 ? (float)currentResource.Value / maxResource.Value : 0;
        Color targetColor = GetThresholdColor(ratio);
        for (int i = 0; i < fillImages.Count; i++)
        {
            fillImages[i].color = targetColor;
            fillImages[i].gameObject.SetActive(i < currentResource.Value);
        }
        if (currentResource.Value != previousResourceValue)
        {
            int affectedIndex = Mathf.Clamp(Mathf.Min(currentResource.Value, previousResourceValue), 0, fillImages.Count -1);
            if (activeBlinkRoutine != null)
                StopCoroutine(activeBlinkRoutine);
            activeBlinkRoutine = StartCoroutine(BlinkEffectRoutine(fillImages[affectedIndex]));
            previousResourceValue = currentResource.Value;
        }
    }
    private Color GetThresholdColor(float ratio)
    {
        if (ratio <= criticalThreshold) return criticalColor;
        if (ratio <= warningThreshold) return warningColor;
        return normalColor;
    }
    private IEnumerator BlinkEffectRoutine(Image targetImage)
    {
        GameObject fillObj = targetImage.gameObject;
        float interval = blinkDuration / (blinkCount * 2);

        for (int i = 0; i < blinkCount; i++)
        {
            fillObj.SetActive(false);
            yield return new WaitForSeconds(interval);
            fillObj.SetActive(true);
            yield return new WaitForSeconds(interval);
        }
        activeBlinkRoutine = null;
    }
    #endregion
}