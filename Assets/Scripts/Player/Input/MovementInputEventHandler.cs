using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MovementInputEventHandler : MonoBehaviour
{
    public UnityEvent<Vector2> MovementDirectionChanged;
    public UnityEvent<Vector2> LookInputAction;
    public UnityEvent JumpInputAction;

    private Vector2 _movementDirection;
    private Vector2 _lookInputProperty;

    public bool sprintModifier;
    public bool crouchModifier;
    public bool jumpPressed;

    private Vector2 _lookInput
    {
        get
        {
            return _lookInputProperty;
        }
        set
        {
            _lookInputProperty = internalClampLookInput(value);
        }
    }
    private Vector2 internalClampLookInput(Vector2 unclampedLookInput)
    {
        float xVal = unclampedLookInput.x;
        float yVal = unclampedLookInput.y;

        if (xVal < -180)
            xVal += 360;
        else if (xVal > 180)
            xVal -= 360;

        yVal = Mathf.Clamp(yVal, -89, 89);

        return new Vector2(xVal, yVal);
    }

    public Vector2 GetMovementDirectionRaw()
    {
        if (enabled)
            return _movementDirection.normalized;
        else
            return Vector2.zero;
    }
    public Vector3 GetMovementDirectionInTransformSpace(Transform transformSpace)
    {
        if (enabled)
            return (transformSpace.forward * _movementDirection.y + transformSpace.right * _movementDirection.x).normalized;
        else
            return Vector3.zero;
    }

    private void Start()
    {
        _movementDirection = Vector2.zero;
        _lookInput = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        sprintModifier = false;
        crouchModifier = false;
    }

    public void OnMove(InputAction.CallbackContext Context)
    {
        if (enabled)
        {
            _movementDirection = Context.ReadValue<Vector2>();
            MovementDirectionChanged?.Invoke(_movementDirection);
        }
    }

    public void OnLook(InputAction.CallbackContext Context)
    {
        if (enabled)
        {
            Vector2 currentLookInput = _lookInput;
            currentLookInput += Context.ReadValue<Vector2>() * Mathf.Deg2Rad;
            _lookInput = currentLookInput;
            LookInputAction?.Invoke(_lookInput);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            sprintModifier ^= sprintModifier;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        crouchModifier = context.started ? true : context.canceled ? false : crouchModifier;
    }

    public void OnJump(InputAction.CallbackContext context) 
    { 
        if (context.started)
        {
            jumpPressed = true;
        }
    }
}
