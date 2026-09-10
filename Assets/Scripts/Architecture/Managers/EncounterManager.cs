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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct WaveData
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnInterval;
}


public class EncounterManager : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Dependencies")]
    [SerializeField] private PoolManager poolManager;

    [Header("Events")]
    [SerializeField] private GameEvent onEncounterCompleted;
    
    #endregion
    #region Internal
    private EncounterTrigger currentActiveTrigger;
    private int activeEnemyCount = 0;
    private Coroutine activeEncounterCoroutine;
    #endregion

    
    #region Methods
    public void StartTriggerEncounter(EncounterTrigger trigger)
    {
        if (trigger == null) return;
        if (activeEncounterCoroutine != null)
            StopCoroutine(activeEncounterCoroutine);

        currentActiveTrigger = trigger;
        activeEnemyCount = 0;
        activeEncounterCoroutine = StartCoroutine(ProcessTriggerRoutine(trigger));
    }

    public void OnEnemyDied()
    {
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
    }
    #endregion


    #region Routine Logic
    private IEnumerator ProcessTriggerRoutine(EncounterTrigger trigger)
    {
        IReadOnlyList<WaveData> waves = trigger.Waves;
        Transform[] spawnPoints = trigger.SpawnPoints;

        for (int w = 0; w < waves.Count; w++)
        {
            WaveData wave = waves[w];

            if (poolManager != null)
                poolManager.Prewarm(wave.enemyPrefab, wave.count);

            for (int i = 0; i < wave.count; i++)
            {
                SpawnEnemy(wave.enemyPrefab, spawnPoints);
                yield return new WaitForSeconds(wave.spawnInterval);
            }
            yield return new WaitUntil(() => activeEnemyCount <=0);
        }
        trigger.CompleteTrigger();
        currentActiveTrigger = null;

        if (onEncounterCompleted != null) onEncounterCompleted.Raise();
    }

    private void SpawnEnemy(GameObject enemyprefab, Transform[] points)
    {
        if (poolManager == null || enemyprefab == null || points.Length == 0) return;

        Transform spawnPoint = points[UnityEngine.Random.Range(0, points.Length)];
        var spawnDelegate = poolManager.GetSpawnDelegate(enemyprefab);

        GameObject enemyObj = spawnDelegate.Invoke(spawnPoint.position, spawnPoint.rotation);

        if (enemyObj.TryGetComponent<EntityBase>(out var entity))
        {
            entity.InitializeEntity(poolManager, enemyprefab);
            activeEnemyCount++;
        }
    }
    #endregion
}