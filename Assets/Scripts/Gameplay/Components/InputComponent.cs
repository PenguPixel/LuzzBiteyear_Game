#region Project Details
/*
* Project: LuzzBiteyear
* Author:Philipp Locher / pengupixels.de
* Issue: Link: https://github.com/PenguPixel/LuzzBiteyear_Game/issues/
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
/// This class acts as the central hub for reading player input from the Input System and dispatching actions to other specialized components. It coordinates movement, attacking, shooting, etc., based on current inputs.
/// </para>
/// </remarks>
/// <summary>
/// Description: The InputComponent reads raw input values (like directional sticks or button presses) using Unity's Input Actions system. It translates these abstract inputs into concrete commands (e.g., "Move left," "Jump") and forwards them to the appropriate systems, such as MovementComponent, ShootingComponent, etc.
/// Coordination: This component primarily coordinates by calling public methods on other dependent components (MovementComponent, TargetingComponent, AttackComponent, ShootingComponent) when input changes occur within Update().
/// Deployment: This script must be attached to the root GameObject that receives player input (e.g., the Player character). It relies heavily on properly configured Input Action Assets assigned in the Inspector.
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
    [Header("Gamestate")]
    [SerializeField] private GameStateVariable currentGameState;

    [Header("Component References")]
    [SerializeField] private MovementComponent movementComponent;
    [SerializeField] private PlayerTargeting playerTargeting;
    [SerializeField] private AttackComponent attackComponent;
    [SerializeField] private ShootingComponent shootingComponent;
    [SerializeField] private InteractionComponent interactionComponent;
    [SerializeField] private GameManager gameManager;

    [Header("Movement Actions")]
    [SerializeField] private InputActionProperty MoveAction;
    [SerializeField] private InputActionProperty JumpAction;
    [Header("Combat Actions")]
    [SerializeField] private InputActionProperty AttackAction;
    [SerializeField] private InputActionProperty ShootAction;
    [SerializeField] private InputActionProperty TargetNextAction;
    [SerializeField] private InputActionProperty TargetPreviousAction;
    [Header("Interaction Actions")]
    [SerializeField] private InputActionProperty InteractAction;
    [SerializeField] private InputActionProperty PauseAction;

    #endregion
    #region Internal
    private Vector3 _currentMoveInput;
    #endregion

    #region UnityMethods  

    public void OnEnable()
    {
        MoveAction.action?.Enable();
        JumpAction.action?.Enable();
        AttackAction.action?.Enable();
        ShootAction.action?.Enable();
        TargetNextAction.action?.Enable();
        TargetPreviousAction.action?.Enable();
        InteractAction.action?.Enable();
        PauseAction.action?.Enable();
    }

    public void OnDisable()
    {
        MoveAction.action?.Disable();
        JumpAction.action?.Disable();
        AttackAction.action?.Disable();
        ShootAction.action?.Disable();
        TargetNextAction.action?.Disable();
        TargetPreviousAction.action?.Disable();
        InteractAction.action?.Disable();
        PauseAction.action?.Disable();
    }

    public void Update()
    {
        HandlePauseInput();

        if (currentGameState != null && currentGameState.Value != GameState.Exploration && currentGameState.Value != GameState.Combat)
        {
            movementComponent?.SetMoveValue(Vector3.zero);
            return;
        }

        // Handle interact input
        HandleInteractInput();

        // Handle movement input
        HandleMoveInput();

        // Handle jump input
        HandleJumpInput();

        // Handle shoot input
        HandleShootInput();

        // Handle attack input
        HandleAttackInput();

        // Handle targeting input
        HandleNextTargetInput();
        HandlePreviousTargetInput();

        
    }

    #endregion

    #region Methods
    /// <summary>
    /// Brief description of the method.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>

    private void HandleInteractInput()
    {
        if (InteractAction.action != null && InteractAction.action.WasPressedThisFrame())
        {
            interactionComponent?.ExecuteInteraction();
        }
    }
    private void HandleShootInput()
    {
        if (ShootAction.action != null && ShootAction.action.WasPressedThisFrame())
        {
            shootingComponent?.ExecuteFire();
        }
    }

    private void HandleAttackInput()
    {
        if (AttackAction.action != null && AttackAction.action.WasPressedThisFrame())
        {
            attackComponent?.ExecuteAttack();
        }
    }

    private void HandleJumpInput()
    {
        if (JumpAction.action != null && JumpAction.action.WasPressedThisFrame())
        {
            movementComponent?.RequestJump();
        }
    }

    private void HandleMoveInput()
    {
        if (MoveAction.action != null)
        {
            Vector2 moveActionValue = MoveAction.action.ReadValue<Vector2>();
            _currentMoveInput = new Vector3(moveActionValue.x, 0f, moveActionValue.y);
            movementComponent?.SetMoveValue(_currentMoveInput);
        }
    }

    private void HandleNextTargetInput()
    {
        if (playerTargeting == null) return;
        if (TargetNextAction.action != null && TargetNextAction.action.WasPressedThisFrame())
        {
            playerTargeting.CycleNextTarget();
        }
    }

    private void HandlePreviousTargetInput()
    {
        if (playerTargeting == null) return;
        if (TargetPreviousAction.action != null && TargetPreviousAction.action.WasPressedThisFrame())
        {
            playerTargeting.CyclePreviousTarget();
        }
    }

    private void HandlePauseInput()
    {
        if (gameManager == null) return;
        if (PauseAction.action != null && PauseAction.action.WasPressedThisFrame())
        {
            gameManager.TogglePause();
        }
    }

    #endregion
}