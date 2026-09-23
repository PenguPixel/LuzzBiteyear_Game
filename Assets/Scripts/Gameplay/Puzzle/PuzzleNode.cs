#region Project Details
/*
* Project: LuzzBiteyear
* Author:Philipp Locher / pengupixels.de
* Issue: Link: https://github.com/PenguPixel/LuzzBiteyear_Game/issues/
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
using UnityEngine.UI;


public class PuzzleNode : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif

    [Header("Rotation Steps")]
    [Tooltip("0 = 0°, 1 = 90°, 2 = 180°, 3 = 270°")]
    [SerializeField] private int[] validSteps = new int[] {0};    
    #endregion

    #region Internal
    private int _currentStep = 0;
    private Button _button;
    public bool IsConnected {get; private set;}
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Rotate);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    public void Scramble()
    {
        int attempts = 0;
        do
        {
            _currentStep = Random.Range(1, 4);
            Evaluate();
            attempts++;            
        }
        while (IsConnected && attempts < 10);

        ApplyRotation();
    }

    public void Rotate()
    {
        _currentStep = (_currentStep +1) % 4;
        ApplyRotation();
        Evaluate();
        PuzzleController.Instance.CheckState();
    }

    private void ApplyRotation()
    {
        transform.localEulerAngles = new Vector3(0f, 0f, _currentStep * -90f);
    }

    private void Evaluate()
    {
        IsConnected = false;

        foreach (int valid in validSteps)
        {
            if (_currentStep == valid)
            {
                IsConnected = true;
                break;
            }
        }
    }
    #endregion
}