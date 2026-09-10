#region Project Details
/*
* Project: MyProjectName
* Author:Philipp / developer@domain.com
* Issue: Link: https://github.com/Wasted-Resources/MyProjectName/issues/[ID]
* Date: 2026-09-10
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
using UnityEngine.InputSystem;


public class InputComponent : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Component References")]
    [SerializeField] private MovementComponent movementComponent;
    [SerializeField] private TargetingComponent targetingComponent;
    [SerializeField] private AttackComponent attackComponent;
    [SerializeField] private ShootingComponent shootingComponent;

    [Header("Input Actions")]
    [SerializeField] private InputActionProperty MoveAction;
    [SerializeField] private InputActionProperty JumpAction;
    [SerializeField] private InputActionProperty AttackAction;
    [SerializeField] private InputActionProperty ShootAction;
    [SerializeField] private InputActionProperty TargetAction;
    [SerializeField] private InputActionProperty InteractAction;

    #endregion
    #region Internal
    private Vector3 _currentMoveInput;
    
    #endregion

    
    #region Methods
    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    public void OnEnable()
    {
        MoveAction.action?.Enable();
        JumpAction.action?.Enable();
        AttackAction.action?.Enable();
        ShootAction.action?.Enable();
        TargetAction.action?.Enable();
        InteractAction.action?.Enable();
    }

    public void OnDisable()
    {
        MoveAction.action?.Disable();
        JumpAction.action?.Disable();
        AttackAction.action?.Disable();
        ShootAction.action?.Disable();
        TargetAction.action?.Disable();
        InteractAction.action?.Disable();
    }

    public void Update()
    {
        // Handle movement input
        if (MoveAction.action != null)
        {
            Vector2 moveActionValue = MoveAction.action.ReadValue<Vector2>();
            _currentMoveInput = new Vector3(moveActionValue.x, 0f, moveActionValue.y);
            movementComponent?.SetMoveValue(_currentMoveInput);
        }

        // Handle jump input
        if (JumpAction.action != null && JumpAction.action.WasPressedThisFrame())
        {
            movementComponent?.RequestJump();
        }

        // Handle shoot input
        if (ShootAction.action != null && ShootAction.action.WasPressedThisFrame())
        {
            shootingComponent?.ExecuteFire();
        }
    }

    #endregion
}