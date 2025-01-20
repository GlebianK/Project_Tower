using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnLook : MonoBehaviour
{
    [SerializeField] private MovementInputEventHandler movementHandler;
    [SerializeField] private Transform cameraFollow;

    private void OnEnable()
    {
        movementHandler.LookInputAction.AddListener(OnLookChanged);
    }
    private void OnDisable()
    {
        movementHandler.LookInputAction.RemoveListener(OnLookChanged);
    }

    private void OnLookChanged(Vector2 lookInput)
    {
        float xSway = lookInput.x;
        float ySway = lookInput.y;

        transform.rotation = Quaternion.Euler(Vector3.up * xSway);
        cameraFollow.transform.localRotation = Quaternion.Euler(Vector3.right * ySway); 
    }
}
