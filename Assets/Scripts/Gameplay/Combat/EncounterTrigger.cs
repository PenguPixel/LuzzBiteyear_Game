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
/// This class handles [Core Responsibility]. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: [Describe what this class does].
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: [Where it should live in the Scene, Project, Assets'].
/// </summary>
#endregion


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    #endregion
}