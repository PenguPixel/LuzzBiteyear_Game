#region Project Details
using System.Net.Sockets;
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-23
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
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public enum CactusType
{
    Direct,
    Harvest
}
public class InteractableCactus : MonoBehaviour, IInteractable
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Behaviour Settins")]
    [SerializeField] private InteractionData interactionData;
    [SerializeField] private CactusType cactusType = CactusType.Direct;
    [SerializeField] private int healAmount = 1;
    [SerializeField] private float cooldownTime = 60f;

    [Header("Visuals")]
    [SerializeField] private List<GameObject> fruitObjects = new();

    [Header("Audio Feedback")]
    [SerializeField] private AudioEventChannel audioChannel;
    [SerializeField] private SoundData soundData;
    
    #endregion


    #region Internal
    private bool isInteractable;
    private int currentFruitCount;
    private Coroutine cooldownRoutine;

    public InteractionData InteractionData => interactionData;

    private void Awake()
    {
        currentFruitCount = fruitObjects.Count;
    }
    private void OnEnable()
    {
        UpdateVisuals();
    }
    #endregion


    #region Methods
    public void Interact(GameObject interactor)
    {
        if (!isInteractable) return;

        if (interactor.CompareTag("Player") && interactor.TryGetComponent(out Health health))
            health.Heal(healAmount);

        if (audioChannel != null && soundData != null)
            audioChannel.RaiseSFX(soundData, transform.position);
        if (cactusType == CactusType.Harvest)
        {
            Harvest();
        }
        else
        {
            isInteractable = false;
            if (cooldownRoutine != null) StopCoroutine(cooldownRoutine);
            cooldownRoutine = StartCoroutine(CooldownRoutine());
        }
    }
    private void Harvest()
    {
        if (currentFruitCount <= 0) return;
        currentFruitCount--;
        UpdateVisuals();
        if (currentFruitCount == 0)
            isInteractable = false;
        if (cooldownRoutine == null)
            cooldownRoutine = StartCoroutine(RegrowRoutine());
        
    }

    private IEnumerator RegrowRoutine()
    {
        while(currentFruitCount < fruitObjects.Count)
        {
            yield return new WaitForSeconds(cooldownTime);
            currentFruitCount++;
            UpdateVisuals();
            isInteractable = true;
        }
        cooldownRoutine = null;
    }
    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        isInteractable = true;
        cooldownRoutine = null;
    }
    private void UpdateVisuals()
    {
        if (cactusType != CactusType.Harvest) return;
        for (int i = 0; i < fruitObjects.Count; i++)
        {
            if (fruitObjects[i] != null)
                fruitObjects[i].SetActive(i < currentFruitCount);
        }
    }

    // Testing
    [ContextMenu("Test Interact")]
    private void TestInteract()
    {
        Interact(gameObject);
    }
    #endregion
}