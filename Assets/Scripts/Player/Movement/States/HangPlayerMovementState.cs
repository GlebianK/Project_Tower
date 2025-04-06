using UnityEngine;
using UnityEngine.Windows;

public class HangPlayerMovementState : PlayerMovementStateBase
{
    private HangRail rail;

    private Vector3 targetPos;

    private IKHangRail ikHandPlaceRoot;

    public HangPlayerMovementState(PlayerMovementStateConfig config) : 
        base(config)
    {
        rail = null;
        ikHandPlaceRoot = GameObject.FindAnyObjectByType<IKHangRail>();
        ikHandPlaceRoot.gameObject.SetActive(false);
    }

    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        Vector3 position = movementController.transform.position
            - (rail.forward * config.HangBodyOffset.z
                + rail.right * config.HangBodyOffset.x
                + Vector3.up * config.HangBodyOffset.y);

        position = rail.GetPositionOnRail(position);

        rail = rail.GetRailSegment(position);
        targetPos = position +
            (rail.forward * config.HangBodyOffset.z
            + rail.right * config.HangBodyOffset.x
            + Vector3.up * config.HangBodyOffset.y);

        ikHandPlaceRoot.transform.position = position;
        ikHandPlaceRoot.transform.rotation = Quaternion.LookRotation(rail.forward, Vector3.up);
        Vector3 input = inputController.GetMovementDirectionInTransformSpace(movementController.transform);

        Vector3 wsInput = Vector3.Project(input, rail.right).normalized;
        float dot = Vector3.Dot(rail.right, wsInput);

        ikHandPlaceRoot.SetRightMovementDirectionModifier(dot);


        movementController.transform.position = Vector3.MoveTowards(
            movementController.transform.position,
            targetPos,
            config.HangSnapSpeed * deltaTime);
    }

    public override bool MakeTransitions(float deltaTime)
    {
        if (inputController.jumpPressed)
        {
            Vector3 forwardXZ = movementController.transform.forward;
            forwardXZ.y = 0;
            forwardXZ = forwardXZ.normalized;

            movementController.Jump(Vector3.up * config.HangJumpVelocity - Mathf.Clamp01(Vector3.Dot(forwardXZ, -rail.forward)) * rail.forward * config.HangHorizontalSpeed);
            movementController.SetCurrentState(PlayerMovementStateType.Air);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        return false;
    }

    public override void OnStateActivated (IPlayerMovementState prevState)
    {
        Vector3 position = movementController.transform.position 
            +movementController.transform.up * config.HangCheckOffset.y;

        Vector3 direction = movementController.transform.forward * config.HangCheckOffset.z
            + movementController.transform.right * config.HangCheckOffset.x;
        if (Physics.Raycast(position,
            direction.normalized, 
            out RaycastHit hit,
            direction.magnitude,
            config.HangDetectionLayer, QueryTriggerInteraction.Collide))
        {
            rail = hit.transform.GetComponent<HangRail>();
            if (rail == null)
            {
                movementController.SetCurrentState(PlayerMovementStateType.Air);
                inputController.jumpPressed = false;
                return;
            }

            position -= movementController.transform.forward * config.HangBodyOffset.z
                + movementController.transform.right * config.HangBodyOffset.x
                + movementController.transform.up * config.HangBodyOffset.y;

            position = rail.GetPositionOnRail(position);
            targetPos = position;

            ikHandPlaceRoot.transform.position = position;
            ikHandPlaceRoot.transform.rotation = Quaternion.LookRotation(rail.forward, Vector3.up);
            ikHandPlaceRoot.gameObject.SetActive(true);

            //movementController.transform.rotation = Quaternion.LookRotation(rail.forward, Vector3.up);
            movementController.cc.enabled = false;

            inputController.jumpPressed = false;
        }
        else
        {
            movementController.SetCurrentState(PlayerMovementStateType.Air);
            inputController.jumpPressed = false;
        }
    }

    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        movementController.cc.enabled = true;
        ikHandPlaceRoot.gameObject.SetActive(false);
    }

    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        Vector2 input = inputController.GetMovementDirectionRaw();

        Vector3 leftRightInput = input.x * rail.right;
        Vector3 forwardInput = Vector3.Project(input.y * movementController.transform.forward, rail.right);

        Vector3 transformInput = Vector3.ClampMagnitude(forwardInput + leftRightInput, 1);

        if (transformInput == Vector3.zero)
            return Vector3.zero;
        return transformInput * config.HangHorizontalSpeed;
    }
}
