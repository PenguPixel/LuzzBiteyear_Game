#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-08
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


public abstract class ResourceComponent<T> : MonoBehaviour, IChangeValue<T>, ISetValue<T>
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] protected string resourceName ;
    [SerializeField] protected GameEvent onValueChanged;
    [SerializeField] protected GameEvent onResourceDepleted;
    #endregion


    #region Methods
    public abstract void ApplyChange(T amount);
    public abstract void SetValue(T amount);
    #endregion
}

[AddComponentMenu("Resources/Float Resource")]
public class FloatResource : ResourceComponent<float>
{
    [SerializeField] private FloatReference currentResource;
    [SerializeField] private FloatReference maxResource;

    #region Overrides
    public override void ApplyChange(float amount)
    {
        if (currentResource == null) return;
        float max = maxResource != null ? maxResource.Value : float.MaxValue;
        currentResource.Value = Mathf.Clamp(currentResource.Value + amount, 0f, max);
        if (onValueChanged != null) onValueChanged.Raise();
        if (currentResource.Value <= 0f && onResourceDepleted != null) onResourceDepleted.Raise();
    }
    public override void SetValue(float newValue)
    {
        if (currentResource == null) return;
        float max = maxResource != null ? maxResource.Value : float.MaxValue;
        currentResource.Value = Mathf.Clamp(newValue, 0f, max);
        if (onValueChanged != null) onValueChanged.Raise();
    }
    #endregion
}

[AddComponentMenu("Resources/Int Resource")]
public class IntResource : ResourceComponent<int>
{
    [SerializeField] private IntReference currentResource;
    [SerializeField] private IntReference maxResource;

    #region Overrides
    public override void ApplyChange(int amount)
    {
        if (currentResource == null) return;
        int max = maxResource != null ? maxResource.Value : int.MaxValue;
        currentResource.Value = Mathf.Clamp(currentResource.Value + amount, 0, max);
        if (onValueChanged != null) onValueChanged.Raise();
        if (currentResource.Value <= 0 && onResourceDepleted != null) onResourceDepleted.Raise();
    }
    public override void SetValue(int newValue)
    {
        if (currentResource == null) return;
        int max = maxResource != null ? maxResource.Value : int.MaxValue;
        currentResource.Value = Mathf.Clamp(newValue, 0, max);
        if (onValueChanged != null) onValueChanged.Raise();
    }
    #endregion
}