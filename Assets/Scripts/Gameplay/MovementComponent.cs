#region Project Details
/*
* Project: MyProjectName
* Author:DeveloperName / developer@domain.com
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
using UnityEngine.InputSystem;


public class MovementComponent : MonoBehaviour
{
    #region Inspector
#if UNITY_EDITOR
    [TextArea] public string DeveloperDescription = string.Empty ;
#endif
    [Header("Movement Settings")]
    [SerializeField] private FloatReference MaxMoveSpeed;
    [SerializeField] private FloatReference CurrentMoveSpeed;
    [SerializeField] private float RotationSpeed = 10f;

    [Header("Input Actions")]
    [SerializeField] private InputActionProperty MoveAction;  
    #endregion

    #region Internal
    private Rigidbody _rigidbody;
    private Vector2 _currentMoveInput;
    #endregion

    #region For Debugging
    private float MoveSpeed = 5f;
    private float _currentMoveSpeed;
    #endregion

    
    #region Methods
    /// <summary>
    /// This method checks the current state of the character object and uses values from the CharacterController Component to set movement values to the character object.
    /// </summary>
    /// <param name = "parameters">What this parameter represents </param>
    // public void GoodMethod(int parameters)
    // {
    //     /* --- CodeBlock: Logic Execution --- */
    //     // Description: Describe the intent of this specific block
    //     var value = parameters * 2;   // Descriptive comment for specific line, if necessary
    // }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;

        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnEnable()
    {
        MoveAction.action?.Enable();
    }

    private void OnDisable()
    {
        MoveAction.action?.Disable();
    }

    private void Update()
    {
        if(MoveAction != null)
        {
            _currentMoveInput = MoveAction.action.ReadValue<Vector2>();
            Debug.Log($"Current Move Input: {_currentMoveInput}");
        }
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(_currentMoveInput.x, 0f, _currentMoveInput.y);

        float targetSpeed = MaxMoveSpeed != null ? MaxMoveSpeed.Value : 5f;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            moveDirection.Normalize();

            // Turn Character towards movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime);

            // set/track current move speed
            // if (CurrentMoveSpeed != null)
            // {
            //     CurrentMoveSpeed.Value = targetSpeed;
            // }

            _currentMoveSpeed = MoveSpeed;
        }
        else
        {
            // if (CurrentMoveSpeed != null)
            // {
            //     CurrentMoveSpeed.Value = 0f;
            // }
            _currentMoveSpeed = 0f;
        }

        // apply linear velocity
        Vector3 targetVelocity = moveDirection * _currentMoveSpeed;
        _rigidbody.linearVelocity = new Vector3(targetVelocity.x, _rigidbody.linearVelocity.y, targetVelocity.z);
    }



    #endregion
}
