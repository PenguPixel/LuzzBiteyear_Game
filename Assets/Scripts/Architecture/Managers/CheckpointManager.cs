#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-26
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


public class CheckpointManager : MonoBehaviour
{
    #region Inspector
    [Header("Default Setup")]
    [SerializeField] private Transform defaultSpawnPoint;
    [SerializeField] private GameObject playerObject;

    [Header("Broadcasting Events")]
    [SerializeField] private GameEvent onCheckpointSaved;
    #endregion


    #region Internal
    private Vector3 lastCheckpointPosition;
    private Quaternion lastCheckpointRotation;
    private bool hasCheckpoint = false;

    private void Awake()
    {
        if (defaultSpawnPoint != null)
        {
            SaveCheckpoint(defaultSpawnPoint.position, defaultSpawnPoint.rotation);
        }
    }
    #endregion


    #region Methods
    public void SaveCheckpoint(Vector3 position, Quaternion rotation)
    {
        lastCheckpointPosition = position;
        lastCheckpointRotation = rotation;
        hasCheckpoint = true;

        if (onCheckpointSaved != null) onCheckpointSaved.Raise();
    }
    #endregion


    #region Event Handlers
    public void OnRequestRespawn()
    {
        if (playerObject == null) return;

        playerObject.SetActive(true);

        Vector3 spawnPos = hasCheckpoint ? lastCheckpointPosition : (defaultSpawnPoint != null ? defaultSpawnPoint.position : Vector3.zero);
        Quaternion spawnRot = hasCheckpoint ? lastCheckpointRotation : (defaultSpawnPoint != null ? defaultSpawnPoint.rotation : Quaternion.identity);

        playerObject.transform.SetPositionAndRotation(spawnPos, spawnRot);

        if (playerObject.TryGetComponent<Health>(out var health))
        {
            health.Heal(3); // why it still heals to full life is beyond what i can fix now
        }
    }
    #endregion
}