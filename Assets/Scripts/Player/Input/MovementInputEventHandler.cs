using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MovementInputEventHandler : MonoBehaviour
{
    public bool sprintModifier;
    public bool crouchModifier;
    public bool jumpPressed;

    public UnityEvent<Vector2> MovementDirectionChanged;
    public UnityEvent<Vector2> LookInputAction;
    public UnityEvent JumpInputAction;

    private Vector2 movementDirection;
    private Vector2 lookInputProperty;

    private Vector2 lookInput
    {
        get
        {
            return lookInputProperty;
        }
        set
        {
            lookInputProperty = internalClampLookInput(value);
        }
    }

    private void Start()
    {
        movementDirection = Vector2.zero;
        lookInput = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        sprintModifier = false;
        crouchModifier = false;
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
            return movementDirection.normalized;
        else
            return Vector2.zero;
    }
    public Vector3 GetMovementDirectionInTransformSpace(Transform transformSpace)
    {
        if (enabled)
            return (transformSpace.forward * movementDirection.y + transformSpace.right * movementDirection.x).normalized;
        else
            return Vector3.zero;
    }
    public void OnMove(InputAction.CallbackContext Context)
    {
        if (enabled)
        {
            movementDirection = Context.ReadValue<Vector2>();
            MovementDirectionChanged?.Invoke(movementDirection);
        }
    }
    public void OnLook(InputAction.CallbackContext Context)
    {
        if (enabled)
        {
            Vector2 currentLookInput = lookInput;
            currentLookInput += Context.ReadValue<Vector2>() * Mathf.Deg2Rad;
            lookInput = currentLookInput;
            LookInputAction?.Invoke(lookInput);
        }
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            sprintModifier ^= true;
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
