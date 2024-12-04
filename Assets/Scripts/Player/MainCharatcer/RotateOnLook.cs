using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnLook : MonoBehaviour
{
    [SerializeField]
    private MovementInputEventHandler _movementHandler;

    [SerializeField]
    private Transform _cameraFollow;

    private void OnEnable()
    {
        _movementHandler.LookInputAction.AddListener(OnLookChanged);
    }
    private void OnDisable()
    {
        _movementHandler.LookInputAction.RemoveListener(OnLookChanged);
    }

    private void OnLookChanged(Vector2 lookInput)
    {
        float xSway = lookInput.x;
        float ySway = lookInput.y;

        transform.rotation = Quaternion.Euler(Vector3.up * xSway);
        _cameraFollow.transform.localRotation = Quaternion.Euler(Vector3.right * ySway); 
    }
}
