using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralCharacterAnimator : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private PlayerMovementController _moveController;
    [SerializeField] private MovementInputEventHandler _movementInputEventHandler;
    [SerializeField] private float _positionTransitSpeed;
    [SerializeField] private float _rotationTransitAngularSpeed;

    [Header("Sway Properties")]
    [SerializeField] private float _swayAmount = 0.01f;
    [SerializeField] private float _maxSwayAmount = 0.5f;
    [SerializeField] private float _swaySmooth = 50f;
    [SerializeField] private AnimationCurve _swayCurve;

    [Range(0f, 1f)]
    [SerializeField] private float _swaySmoothCounteraction = 1f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSwayMultiplier = -1f;

    [Header("Position")]
    [SerializeField] private float _positionSwayMultiplier = 9f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector2 sway;
    private Quaternion lastRot;

    [Header("Bobbing")]
    [SerializeField] private AnimationCurve _bobLeftRightCurve;
    [SerializeField] private AnimationCurve _bobUpCurve;
    [SerializeField] private AnimationCurve _bobRollAlpha;
    [SerializeField] private AnimationCurve _bobFadeCurve;
    [SerializeField] private float _bobPositionOffsetMultiplier = 1f;

    [Header("Walk Animation Settings")]
    [SerializeField] private Vector3 _walkPosAmount = new Vector3(0.004f, 1, 0.003f);

    private Vector3 _walkAnimationPosition;
    private Vector3 _walkAnimationRotation;
    private float _walkAnimationAlpha;
    private Vector3 _walkAnimLagPos = Vector3.zero;
    private float _walkAnimationTimer;
    private float _walkTimerMaxValue;

    private float _lastFrameUp = 0;

    private void Reset()
    {
        Keyframe[] ks = new Keyframe[] { new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 0, 0) };
        _swayCurve = new AnimationCurve(ks);
        _bobLeftRightCurve = new AnimationCurve(ks);
        _bobUpCurve = new AnimationCurve(ks);
    }

    private void Start()
    {
        if (!_weaponTransform)
            _weaponTransform = transform;
        lastRot = transform.localRotation;
        initialPosition = _weaponTransform.localPosition;
        initialRotation = _weaponTransform.localRotation;
        _walkAnimLagPos = Vector3.zero;
        StartCoroutine(WalkTimeline());
        _weaponScale = _weaponTransform.localScale;
    }

    private void Update()
    {
        
        Vector3 oldWeaponPosition = _weaponTransform.localPosition;
        Quaternion oldWeaponRotation = _weaponTransform.localRotation;

        WalkAnimationUpdateVectors();
        UpdateWalkAnimationLag();
        TransformWeapon();

        Sway();


        Vector3 newWeaponPosition = Vector3.Lerp(
            _weaponTransform.localPosition,
            _weaponTransform.localPosition + new Vector3(sway.x, sway.y, 0) * _positionSwayMultiplier * Mathf.Deg2Rad,
            _swayCurve.Evaluate(Time.deltaTime * _swaySmooth));

        Quaternion newWeaponRotation = Quaternion.Slerp(
            _weaponTransform.localRotation,
            _weaponTransform.localRotation * Quaternion.Euler(Mathf.Rad2Deg * _rotationSwayMultiplier * new Vector3(-sway.y, sway.x, 0)),
            _swayCurve.Evaluate(Time.deltaTime * _swaySmooth));

        _weaponTransform.localPosition = Vector3.MoveTowards(oldWeaponPosition, newWeaponPosition, _positionTransitSpeed * Time.deltaTime);
        _weaponTransform.localRotation = Quaternion.RotateTowards(oldWeaponRotation, newWeaponRotation, _rotationTransitAngularSpeed * Time.deltaTime);
    }

    private void WalkAnimationUpdateVectors()
    {
        float leftRightAlpha = _bobLeftRightCurve.Evaluate(_walkAnimationTimer);
        float UpAlpha = _bobUpCurve.Evaluate(_walkAnimationTimer);
        float rollAlpha = _bobRollAlpha.Evaluate(_walkAnimationTimer);

        _walkAnimationPosition.x = _walkPosAmount.x * Mathf.Lerp(-1, 1, leftRightAlpha);
        _walkAnimationPosition.z = 0;
        _walkAnimationPosition.y = _walkPosAmount.y * Mathf.Lerp(-1, 1, UpAlpha);

        _walkAnimationRotation.x = 0;
        _walkAnimationRotation.y = 0;
        _walkAnimationRotation.z = _walkPosAmount.z * Mathf.Lerp(1, -1, rollAlpha);

        Vector3 xzVelocity = _moveController.CharacterVelocity;
        xzVelocity.y = 0;
        bool isPlayerGrounded = _moveController.IsGrounded;
        if (isPlayerGrounded 
            && _movementInputEventHandler.GetMovementDirectionRaw() != Vector2.zero 
            && !_moveController.State.Equals(PlayerMovementState.Slide))
        {
            float velocity = _moveController.CharacterVelocity.magnitude;
            _walkAnimationAlpha = (velocity / _moveController.MaxSpeed);
        }
        else 
        { 
            _walkAnimationAlpha = 0;
        }
    }

    private void UpdateWalkAnimationLag()
    {
        float forward = Vector3.Dot(_moveController.CharacterVelocity, transform.forward);
        float up = Vector3.Dot(_moveController.CharacterVelocity, transform.up);
        float right = Vector3.Dot(_moveController.CharacterVelocity, transform.right);

        forward = forward / -_moveController.MaxSpeed;
        right = right / _moveController.MaxSpeed;
        up = Mathf.Lerp(_lastFrameUp, -up / _moveController.JumpVelocity, 0.3f);
        _lastFrameUp = up;

        Vector3 lagPos = new Vector3(right, up, forward) * 0.02f;
        lagPos = Vector3.ClampMagnitude(lagPos, 0.04f);
        _walkAnimLagPos = Vector3.MoveTowards(_walkAnimLagPos, lagPos, 1.0f/6.0f);

    }

    private void TransformWeapon()
    {
        Vector3 walkOffsetPos = _walkAnimationPosition * _walkAnimationAlpha;
        Quaternion walkOffsetRot = Quaternion.Slerp(Quaternion.identity,
            Quaternion.Euler(_walkAnimationRotation * 500), _walkAnimationAlpha);

        Vector3 lagPosOffsetRot = Vector3.forward * 2 * _walkAnimLagPos.x;

        _weaponTransform.localPosition = initialPosition + (walkOffsetPos + _walkAnimLagPos + Vector3.right * _walkAnimLagPos.x * 0.5f) * _bobPositionOffsetMultiplier;
        _weaponTransform.localRotation = initialRotation * walkOffsetRot;
        _weaponTransform.localRotation *= Quaternion.Euler(lagPosOffsetRot);
    }

    private IEnumerator WalkTimeline()
    {
        _walkAnimationTimer = 0;
        _walkTimerMaxValue = _bobUpCurve.keys[_bobUpCurve.length - 1].time;
        while (true)
        {
            _walkAnimationTimer += Time.deltaTime;
            while (_walkAnimationTimer > _walkTimerMaxValue)
                _walkAnimationTimer -= _walkTimerMaxValue;

            yield return null;
        }
    }

    private void Sway()
    {
        var angularVelocity = Quaternion.Inverse(lastRot) * transform.rotation;

        float mouseX = FixAngle(angularVelocity.eulerAngles.y) * _swayAmount;
        float mouseY = -FixAngle(angularVelocity.eulerAngles.x) * _swayAmount;

        lastRot = transform.rotation;

        sway = Vector2.MoveTowards(sway, Vector2.zero, _swayCurve.Evaluate(Time.deltaTime * _swaySmoothCounteraction * sway.magnitude * _swaySmooth));
        sway = Vector2.ClampMagnitude(new Vector2(mouseX, mouseY) + sway, _maxSwayAmount);
    }

    private float FixAngle(float angle)
    {
        return Mathf.Repeat(angle + 180f, 360f) - 180f;
    }

    private Vector3 _weaponScale = Vector3.zero;
    public void OnClimbStarted()
    {
        _weaponTransform.localScale = Vector3.one;
    }
    public void OnClimbEnded()
    {
        _weaponTransform.localScale = _weaponScale;
    }
}
