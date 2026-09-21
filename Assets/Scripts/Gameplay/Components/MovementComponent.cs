#region Project Details
/*
* Project: LuzzBiteyear
* Author:Philipp Locher / pengupixels.de
* Issue: Link: https://github.com/PenguPixel/LuzzBiteyear_Game/issues/2
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
/// This class handles character movement and jumping mechanics. It uses the Rigidbody component for physics interactions and input actions for player control.
/// </para>
/// </remarks>
/// <summary>
/// Description: The MovementComponent is responsible for handling the movement and jumping logic of a character in the game. It processes input from the player to move the character forward, backward, left, and right, as well as handle jumping mechanics including single and double jumps.
/// Coordination: This component communicates with other components via Unity's Input System and potentially with animation controllers or health systems to manage state transitions and effects.
/// Deployment: This script should be attached to the GameObject representing the character in the Unity scene. It requires a Rigidbody component and references to various settings and input actions defined in the inspector.
/// </summary>
#endregion


using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterAnimationBridge))]
public class MovementComponent : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif

    [Header("Component References")]
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private CharacterAnimationBridge animationBridge;
    [SerializeField] private TargetingComponent targetingComponent;

    [Header("Movement Settings")]
    [SerializeField] private FloatReference MaxMoveSpeed;
    [SerializeField] private FloatReference CurrentMoveSpeed;
    [SerializeField] private FloatReference RotationSpeed;

    [Header("Jump Settings")]
    [SerializeField] private FloatReference JumpForce;
    [SerializeField] private bool doubleJumpUnlocked = false;

    [Header("Ground Check")]
    [SerializeField] private Transform GroundCheckTransform;
    [SerializeField] private float GroundCheckRadius = 0.2f;
    [SerializeField] private LayerMask GroundLayer;

    #endregion

    #region Public Getters
    public bool IsGrounded => _isGrounded;
    public bool DoubleJumpUnlocked => doubleJumpUnlocked;
    #endregion

    #region Internal   

    // Movement cache fed with values from InputComponent
    private Vector3 _currentMoveInput;
    
    // Jumping
    private bool _isGrounded = true;
    private bool _isJumping = false;
    private bool _jumpRequested = false;
    private int _remainingJumps;
    private int _totalAvailableJumps = 0;

    #endregion

    
    #region Private Methods
    /// <summary>
    /// This method checks the current state of the character object and uses values from the CharacterController Component to set movement values to the character object.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>

    private void Awake()
    {
        if (Rigidbody == null)
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        if (animationBridge == null)
        {
            animationBridge = GetComponent<CharacterAnimationBridge>();

            if (animationBridge == null)
            {
                animationBridge = GetComponentInChildren<CharacterAnimationBridge>();
            }
        }

        Rigidbody.useGravity = true;
        Rigidbody.isKinematic = false;

        Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnEnable()
    {
        if (animationBridge != null)
        {
            animationBridge.OnFootstep += HandleFootstepAnimationEvent;
            animationBridge.OnJumpImpulse += HandleJumpImpulseAnimationEvent;
        }
    }

    private void OnDisable()
    {
        if (animationBridge != null)
        {
            animationBridge.OnFootstep -= HandleFootstepAnimationEvent;
            animationBridge.OnJumpImpulse -= HandleJumpImpulseAnimationEvent;
        }
    }

    private void Update()
    {
        _totalAvailableJumps = doubleJumpUnlocked ? 2 : 1;


        // check if Grounded
        CheckGroundedState();
    }

    
    private void FixedUpdate()
    {
        Move();
        Jump();
    }
    #endregion

    #region Movement Methods
    private void Move()
    {
        Vector3 moveDirection = SetMoveDirection(_currentMoveInput);
        // apply linear velocity
        ApplyMovement(moveDirection);

        if (animationBridge != null)
        {
            float targetSpeed = CurrentMoveSpeed != null ? CurrentMoveSpeed.Value : 0f;
            animationBridge.UpdateLocomotion(targetSpeed);
        }
    }

    private Vector3 SetMoveDirection(Vector3 moveInputValue)
    {
        Vector3 moveDirection = moveInputValue;

        float targetSpeed = MaxMoveSpeed != null ? MaxMoveSpeed.Value : 5f;

        // Focus to Target if a target is acquired
        if (targetingComponent != null && targetingComponent.HasValidTarget)
        {
            Vector3 aimDirection = (targetingComponent.TargetTransform.position - transform.position).normalized;
            aimDirection.y = 0f;

            if (aimDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(aimDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed.Value * Time.fixedDeltaTime);
            }
        }
        
        // Standard movement if no target is acquired
        else if (moveDirection.sqrMagnitude > 0.01f)
        {
            moveDirection.Normalize();

            // Turn Character towards movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed.Value * Time.fixedDeltaTime);

            // set/track current move speed
            if (CurrentMoveSpeed != null)
            {
                CurrentMoveSpeed.Value = targetSpeed;
            }
        }
        
        // Speed tracking for rigidbody
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            if (CurrentMoveSpeed != null) CurrentMoveSpeed.Value = targetSpeed;
        }
        else
        {
            if (CurrentMoveSpeed != null) CurrentMoveSpeed.Value = 0f;
        }

        return moveDirection;
    }

    private void ApplyMovement(Vector3 moveDirection)
    {
        Vector3 targetVelocity = moveDirection * CurrentMoveSpeed.Value;
        Rigidbody.linearVelocity = new Vector3(targetVelocity.x, Rigidbody.linearVelocity.y, targetVelocity.z);
    }
    #endregion

    #region Jump & Groundcheck
    private void CheckGroundedState()
    {
        if (GroundCheckTransform != null)
        {
            _isGrounded = Physics.CheckSphere(GroundCheckTransform.position, GroundCheckRadius, GroundLayer);
        }
    }

    private void Jump()
    {
        if (_jumpRequested)
        {
            Rigidbody.linearVelocity = new Vector3(Rigidbody.linearVelocity.x, 0f, Rigidbody.linearVelocity.z);
            Rigidbody.AddForce(Vector3.up * JumpForce.Value, ForceMode.Impulse);

            if (animationBridge != null)
            {
                if (_remainingJumps == _totalAvailableJumps - 1)
                {
                    animationBridge.TriggerJump();
                }
                else
                {
                    animationBridge.TriggerDoubleJump();
                }
            }

            _jumpRequested = false;
        }
    }    
    #endregion

    #region Public Methods
    public void SetMoveValue(Vector3 moveInputValue)
    {
        _currentMoveInput = moveInputValue;
    }

    public void RequestJump()
    {
        if (!_isGrounded && !_isJumping) return; // Prevent jump if not grounded

            if (_isGrounded)
            {
                _remainingJumps = _totalAvailableJumps; // Reset remaining jumps when grounded
                _jumpRequested = true;
                _remainingJumps--;

                _isJumping = true;
            }
            else if (_isJumping && _remainingJumps > 0)
            {
                _jumpRequested = true;
                _remainingJumps--;
                _isJumping = false; // Reset jumping state after double jump
            }
    }

    public void UnlockDoubleJump()
    {
        doubleJumpUnlocked = true;
    }
    #endregion

    #region Animation Event Handlers
    private void HandleFootstepAnimationEvent()
    {
        // Event for Audio or SFX
    }

    private void HandleJumpImpulseAnimationEvent()
    {
        // Event for Audio or SFX
    }
    #endregion
}

