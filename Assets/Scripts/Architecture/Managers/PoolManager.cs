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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class PoolManager : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 64;
    
    #endregion


    #region Internal
    private readonly Dictionary<int, IObjectPool<GameObject>> pools = new();

    #endregion

    
    #region Methods
    public void Prewarm(GameObject prefab, int count)
    {
        if (prefab == null) return;

        int key = prefab.GetInstanceID();
        if (!pools.TryGetValue(key, out var pool))
        {
            pool = CreatePool(prefab);
            pools.Add(key, pool);
        }

        List<GameObject> tempArray = new(count);
        for (int i = 0; i < count; i++)
        {
            tempArray.Add(pool.Get());
        }
        for (int i = 0; i < tempArray.Count; i++)
        {
            pool.Release(tempArray[i]);
        }
    }

    public Func<Vector3, Quaternion, GameObject> GetSpawnDelegate(GameObject prefab)
    {
        if (prefab == null) return null;

        int key = prefab.GetInstanceID();
        if (!pools.TryGetValue(key, out var pool))
        {
            pool = CreatePool(prefab);
            pools.Add(key, pool);
        }

        return (pos,rot) =>
        {
            GameObject obj = pool.Get();
            obj.transform.SetPositionAndRotation(pos, rot);
            return obj;
        };
    }

    public void Release(GameObject prefab, GameObject instance)
    {
        if (prefab == null || instance == null) return;

        int key = prefab.GetInstanceID();
        if (pools.TryGetValue(key, out var pool))
        {
            pool.Release(instance);
        }
        else
        {
            Destroy(instance);
        }
    }
    private IObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    #endregion
}