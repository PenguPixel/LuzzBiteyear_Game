#region Project Details
/*
* Project: MyProjectName
* Author:Christof Kloninger / kloningerchristof@gmail.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-16
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
/// This class handles the layout within a panel. It must maintain [Architecture Constraint, e.g., Singleton].
/// </para>
/// </remarks>
/// <summary>
/// Description: takes all the children and arranges them in a radial arc. Note to self: Int values are not very friendly to flaot bars...
/// Coordination: [How it communicates with APIs or other Components].
/// Deployment: Attach this script to the IconContainer of the enemy.
/// </summary>
#endregion


using UnityEngine;

[ExecuteAlways]
public class RadialLayout : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [SerializeField] private float radius = 20f;
    [SerializeField] private float minAngle = -45f;
    [SerializeField] private float maxAngle = 45f;

    #endregion


    #region Internal
    private void OnTransformChildrenChanged() => CalculateRadialPosition();
    private void OnValidate() => CalculateRadialPosition();
    #endregion


    #region Methods
    public void CalculateRadialPosition()
    {
        int count = transform.childCount;
        if (count == 0) return;
        float angleStep = count > 1 ? (maxAngle - minAngle) / (count - 1) : 0f;

        for (int i = 0; i < count; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;
            if (child == null) continue;
            float currentAngle = (count == 1) ? (minAngle + maxAngle) / 2f : minAngle + (i * angleStep);
            float rad = currentAngle * Mathf.Deg2Rad;

            Vector2 pos = new(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * radius);
            child.anchoredPosition = pos;
            child.localRotation = Quaternion.Euler(0, 0, -currentAngle);
        }
    }
    #endregion
}