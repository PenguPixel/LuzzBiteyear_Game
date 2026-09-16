#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-16
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
/// This class handles the actual display of enemy health. It gives visual feedback to the player.
/// </para>
/// </remarks>
/// <summary>
/// Description: It shoves pixels around.
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: Attach this script to the enemy.
/// </summary>
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/World Health UI")]
public class WorldHealthUI : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Dependencies")]
    [SerializeField] private Health targetHealth;
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private GameObject heartPrefab;

    [Header("Positioning")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0, 2f, 0);

    [Header("Visual Feedback Configuration")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color criticalColor;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float blinkInterval = 0.3f;
    
    #endregion


    #region Internal
    private struct HeartSlot
    {
        public Image Frame;
        public Image Fill;   
    }
    private readonly List<HeartSlot> heartSlots = new();
    private Camera mainCamera;
    private Coroutine hideRoutine;
    private Coroutine continuousBinkRoutine;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (worldCanvas != null) worldCanvas.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        if (targetHealth != null)
            targetHealth.OnDied += HandleDeath;
    }
    private void OnDisable()
    {
        if (targetHealth != null)
            targetHealth.OnDied -= HandleDeath;
    }
    private void Start()
    {
        InitializeBar();
    }
    private void LateUpdate()
    {
        if (worldCanvas != null && worldCanvas.gameObject.activeSelf)
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera != null)
            {
                worldCanvas.transform.position = transform.position + worldOffset;
                worldCanvas.transform.rotation = mainCamera.transform.rotation;
            }
        }
    }
    #endregion


    #region Methods
    public void InitializeBar()
    {
        foreach (Transform child in iconContainer)
        {
            Destroy(child.gameObject);
        }
        heartSlots.Clear();

        for (int i = 0; i < targetHealth.MaxHealth; i++)
        {
            GameObject instance = Instantiate(heartPrefab, iconContainer);
            if (instance.TryGetComponent<Image>(out var frameImage) &&
                instance.transform.GetChild(0).TryGetComponent<Image>(out var fillImage))
            {
                heartSlots.Add(new HeartSlot {Frame = frameImage, Fill = fillImage});
            }
        }
    }

    public void UpdateHealthUI(int currentHealth)
    {
        if (worldCanvas == null) return;
        worldCanvas.gameObject.SetActive(true);

        bool isCritical = currentHealth <= 1;
        Color activeColor = isCritical ? criticalColor : normalColor;

        for(int i = 0; i < heartSlots.Count; i++)
        {
            heartSlots[i].Frame.color = activeColor;
            heartSlots[i].Fill.color = activeColor;
            heartSlots[i].Fill.gameObject.SetActive(i < currentHealth);
        }

        if (isCritical && currentHealth > 0)
        {
            if (continuousBinkRoutine == null)
                continuousBinkRoutine = StartCoroutine(ContinuousBlinkRoutine(heartSlots[0]));
        }
        else
        {
            StopBlinking();
        }

        if (hideRoutine != null) StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HideAfterDelayRoutine());
    }

    private void HandleDeath(DamageContext context)
    {
        StopBlinking();
        if (worldCanvas != null) worldCanvas.gameObject.SetActive(false);
    }
    #endregion


    #region Helpers
    private IEnumerator ContinuousBlinkRoutine(HeartSlot slot)
    {
        while (true)
        {
            slot.Fill.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkInterval);
            slot.Fill.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkInterval);
        }
    }
    private void StopBlinking()
    {
        if (continuousBinkRoutine != null)
        {
            StopCoroutine(continuousBinkRoutine);
            continuousBinkRoutine = null;
        }
    }
    private IEnumerator HideAfterDelayRoutine()
    {
        yield return new WaitForSeconds(displayDuration);

        if (targetHealth != null && targetHealth.CurrentHealth > 1)
        {
            worldCanvas.gameObject.SetActive(false);
        }
    }
    #endregion
}