using UnityEngine;

public class ClimbPlayerMovementState : PlayerMovementStateBase
{
    private readonly float maxClimbRange;
    private readonly float climbSpeed;

    RaycastHit climbPointHit;

    private float climbTime;
    private Vector3 movementVector;
    public ClimbPlayerMovementState(PlayerMovementStateConfig config) : base(config)
    {
        maxClimbRange = (Vector3.forward * config.ClimbSnapXZRange + Vector3.up * config.ClimbCheckpointHeight).magnitude;
        climbSpeed = maxClimbRange / config.ClimbMaxDuration;
    }

    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        
    }

    public override bool MakeTransitions(float deltaTime)
    {
        if (climbTime >= config.ClimbMaxDuration)
        { 
            movementController.transform.position = climbPointHit.point;

            movementController.SetCurrentState(PlayerMovementStateType.Crouch);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }
        return false;
    }

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        CharacterController cc = movementController.cc;

        movementController.cc.enabled = false;
        movementController.TryGetClimbPoint(out climbPointHit);

        Vector3 startPosition = cc.transform.position;

        Vector3 startPositionXZ = startPosition;
        startPositionXZ.y = 0;
        Vector3 endPositionXZ = climbPointHit.point;
        endPositionXZ.y = 0;

        Vector3 snapVector = endPositionXZ - startPositionXZ;
        // если игрок слишком далеко для корректного(визуально) клаймба
        if (snapVector.sqrMagnitude > Mathf.Pow(config.ClimbSnapXZRange, 2))
        {
            Vector3 maxSnap = Vector3.ClampMagnitude(snapVector, config.ClimbSnapXZRange);
            snapVector -= maxSnap;

            Vector3 snappedPos = startPosition + snapVector;

            startPosition = snappedPos;
            cc.transform.position = snappedPos;
        }

        movementVector = climbPointHit.point - startPosition;

        float startTimeMinMaxed = 1 - movementVector.magnitude / maxClimbRange;

        climbTime = startTimeMinMaxed * config.ClimbMaxDuration;
    }

    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        movementController.cc.enabled = true;
        movementController.CharacterVelocity = Vector3.zero;

        inputController.jumpPressed = false;
    }

    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        float currentSpeed = climbSpeed;
        climbTime += deltaTime;
        return movementVector.normalized * currentSpeed;
    }
}
