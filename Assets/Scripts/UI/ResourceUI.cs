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


using System.Collections.Generic;
using UnityEngine;


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
    #endregion


    #region Internal
    private readonly List<GameObject> activeObjects = new();
    private void Start()
    {
        InitializeBar();
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
        activeObjects.Clear();

        for (int i = 0; i < maxResource.Value; i++)
        {
            GameObject frameInstance = Instantiate(prefab, transform);
            GameObject fillObject = frameInstance.transform.GetChild(0).gameObject;
            activeObjects.Add(fillObject);
        }
    }
    public void UpdateResoureUI()
    {
        for (int i = 0; i < activeObjects.Count; i++)
        {
            activeObjects[i].SetActive(i < currentResource.Value);
        }
    }
    #endregion
}