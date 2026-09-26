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
/// This class handles EncounterManagement. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: [Describe what this class does].
/// Coordination: Listens to Encounter Triggers and delegates spawning enemies for it.
/// Deployment: [Where it should live in the Scene, Project, Assets'].
/// </summary>
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int count;
}
[Serializable]
public class WaveData
{
    public List<EnemyGroup> enemyGroups = new();
    public float spawnInterval = 1f;
}

public class EncounterManager : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Dependencies")]
    [SerializeField] private PoolManager poolManager;

    [Header("Prewarm Setup")]
    [SerializeField] private List<EnemyGroup> prewarmPools = new();

    [Header("NavMeshSpawning")]
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float minEnemyDistance = 1.5f;
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("Events")]
    [SerializeField] private GameEvent onEncounterCompleted;
    
    #endregion
    #region Internal
    private EncounterTrigger currentActiveTrigger;
    private int activeEnemyCount = 0;
    private Coroutine activeEncounterCoroutine;
    private void Start()
    {
        if (poolManager == null) poolManager = FindFirstObjectByType<PoolManager>();
        PrewarmPools();
    }
    #endregion


    #region Methods
    /// <summary>
    /// Prewarms a designated pool of enemies at scene startup to prevent framedrops during gameplay.
    /// </summary>
    private void PrewarmPools()
    {
        if (poolManager == null) return;
        foreach (var group in prewarmPools)
        {
            if (group.enemyPrefab != null && group.count > 0)
            {
                poolManager.Prewarm(group.enemyPrefab, group.count);
            }
        }
    }
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
        Debug.Log($"Enemy died. Recognized. Active enemy count: {activeEnemyCount}");
    }
    #endregion


    #region Routine Logic
    private IEnumerator ProcessTriggerRoutine(EncounterTrigger trigger)
    {
        IReadOnlyList<WaveData> waves = trigger.Waves;

        for (int w = 0; w < waves.Count; w++)
        {
            WaveData wave = waves[w];

            if (wave.spawnInterval > 0)
                yield return new WaitForSeconds(wave.spawnInterval);

            foreach (var group in wave.enemyGroups)
            {
                if (group.enemyPrefab == null || group.count <= 0) continue;
                if (poolManager != null)
                    poolManager.Prewarm(group.enemyPrefab, group.count);

                for (int i = 0; i < group.count; i++)
                {
                    if (SpawnEnemy(group.enemyPrefab, trigger.transform.position))
                        activeEnemyCount++;
                }
                yield return new WaitForSeconds(wave.spawnInterval);
            }
            yield return new WaitUntil(() => activeEnemyCount <=0);
        }
        trigger.CompleteTrigger();
        currentActiveTrigger = null;

        if (onEncounterCompleted != null) onEncounterCompleted.Raise();
    }

    private bool SpawnEnemy(GameObject enemyprefab, Vector3 centrePoint)
    {
        if (poolManager == null || enemyprefab == null) return false;

        Vector3 validSpawnPos = Vector3.zero;
        bool foundValidPosition = false;
        int maxAttempts = 15;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0;
            Vector3 candidatePoint = centrePoint + randomOffset;

            if (NavMesh.SamplePosition(candidatePoint, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
            {
                if(!Physics.CheckSphere(hit.position, minEnemyDistance, enemyLayerMask))
                {
                    validSpawnPos = hit.position;
                    foundValidPosition = true;
                    break;
                }
            }
        }
        if (!foundValidPosition) return false;

        var spawnDelegate = poolManager.GetSpawnDelegate(enemyprefab);
        GameObject enemyObj = spawnDelegate.Invoke(validSpawnPos, Quaternion.identity);

        if (enemyObj.TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.Warp(validSpawnPos);
            agent.enabled = true;
        }
        if (enemyObj.TryGetComponent<EntityBase>(out var entity))
        {
            entity.InitializeEntity(poolManager, enemyprefab);
        }
        return true;
    }
    #endregion
}