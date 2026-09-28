#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-21
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
/// This class handles Icon asset flips. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: Flips preconfigured Icons].
/// Coordination: reads the targetComponent and with the appropriate field it reads it automatically.
/// Deployment: Is a component on the UI Element. Make sure that the string of the boolNameProperty matches exactly which bool you want to listen to. 
/// </summary>
#endregion

using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IconFlip : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("UI Target")]
    [SerializeField] private Image targetImage ;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    [Header("Binding")]
    [SerializeField] private GameObject targetGameObject;
    [SerializeField] private string targetComponentName;
    [SerializeField] private string boolPropertyName;
    #endregion


    #region Internal
    private Component targetComponent;
    private PropertyInfo cachedProperty;
    private FieldInfo cachedField;
    private bool lastState;

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        CacheReflection();
    }

    private void Update()
    {
        if (targetImage == null || (cachedProperty == null && cachedField == null))
            return;
        bool currentState = ReadBoolValue();

        if (currentState != lastState)
        {
            lastState = currentState;
            UpdateVisuals(currentState);
        }
    }
    #endregion


    #region Methods
    private void CacheReflection()
    {
        if (targetGameObject == null || string.IsNullOrEmpty(boolPropertyName))
            return;
        if (!string.IsNullOrEmpty(targetComponentName))
        {
            targetComponent = targetGameObject.GetComponent(targetComponentName);
        }
        System.Type type = targetComponent.GetType();

        cachedProperty = type.GetProperty(boolPropertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (cachedProperty == null)
        {
            cachedField = type.GetField(boolPropertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        }
        if (cachedProperty == null && cachedField == null)
        {
            Debug.LogError($"IconFlipper: Could not find my stuff");
        }

    }
    private bool ReadBoolValue()
    {
        if (cachedProperty != null)
            return (bool)cachedProperty.GetValue(targetComponent);
        if (cachedField != null)
            return(bool)cachedField.GetValue(targetComponent);
        return false;
    }
    private void UpdateVisuals(bool isActive)
    {
        if (targetImage == null) return;
        Sprite newSprite = isActive ? activeSprite : inactiveSprite;
        if (newSprite != null)
            targetImage.sprite = newSprite;
    }
    #endregion
}