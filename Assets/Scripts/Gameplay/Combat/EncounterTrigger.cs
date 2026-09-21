#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-09
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
/// This class handles Encounter Triggers. Can either be a repeatable encounter or resets on its own after the trigger.
/// </para>
/// </remarks>
/// <summary>
/// Description: Reacts to the player entering the trigger area by calling on the Encounter Manager to deal with spawning enemies.
/// Coordination: Triggers on the player entering its trigger area and finds the Encounter Manager to call upon spawning enemies.
/// Deployment: Can be set from the Encounter Prefab anywhere in the scene.
/// </summary>
#endregion


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[AddComponentMenu("Combat/Encounter Trigger")]
public class EncounterTrigger : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif

    [Header("Trigger Setup")]
    [SerializeField] private bool isOneShot = true;
    [SerializeField] List<WaveData> waves = new();
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] private float cooldownTime = 180f;
    [SerializeField] private Collider triggerCollider;

    [Header("Environment Feedback (Optional)")]
    [SerializeField] private GameObject[] barrierObjects;

    [Header("Events")]
    [SerializeField] private GameEvent onEncounterStarted;
    
    
    #endregion


    #region Internal
    private bool hasTriggered;
    public IReadOnlyList<WaveData> Waves => waves;
    public Transform[] SpawnPoints => spawnPoints;
    #endregion


    #region Methods
    public void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && isOneShot) return;
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            ToggleBarriers(true);
            if (onEncounterStarted != null) onEncounterStarted.Raise();

            EncounterManager manager = FindFirstObjectByType<EncounterManager>();
            if (manager != null)
            {
                manager.StartTriggerEncounter(this);
                if (!isOneShot)
                    StartCoroutine(ResetEncounter());
            }
        }
    }

    public void CompleteTrigger()
    {
        ToggleBarriers(false);
    }
    public void ToggleBarriers(bool state)
    {
        for (int i = 0; i < barrierObjects.Length; i++)
        {
            if (barrierObjects[i] != null)
                barrierObjects[i].SetActive(state);
        }
    }
    public IEnumerator ResetEncounter()
    {
        triggerCollider.enabled = false;
        yield return new WaitForSeconds(cooldownTime);
        triggerCollider.enabled = true;
        hasTriggered = false;
    }
    #endregion
}