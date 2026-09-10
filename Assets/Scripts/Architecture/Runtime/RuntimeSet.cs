#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-07
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
/// This class handles Runtimesets of objects/items. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Creates Sets/Lists of items that may be required by several components.s
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: Lives in assets.
/// </summary>
#endregion


using UnityEngine;
using System.Collections.Generic;

public abstract class RuntimeSet<T> : ScriptableObject
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif  
    [SerializeField] private List<T> items = new();
    
    #endregion
    #region Internal
    public IReadOnlyList<T> Items => items;

    private void OnEnable()
    {
        items.Clear();
    }
    #endregion


    #region Methods
    public void Add(T item)
    {
        if (!items.Contains(item)) items.Add(item);
    }
    public void Remove(T item)
    {
        if (items.Contains(item)) items.Remove(item);
    }
    #endregion
}
#region Implementations
//Set of coordinates, for example Spawnpoints
[CreateAssetMenu(menuName = "Sets/Transform Set")]
public class TransformSet : RuntimeSet<Transform> {}

// Set of GameObjects, generally useful
[CreateAssetMenu(menuName = "Sets/GameObject Set")]
public class GameObjectSet : RuntimeSet<GameObject> {}
#endregion