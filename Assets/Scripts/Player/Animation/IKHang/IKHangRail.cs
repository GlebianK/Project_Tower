using System.Collections.Generic;
using UnityEngine;

public class IKHangRail : MonoBehaviour
{
    [SerializeField] private List<IKHangFollower> iks;


    private float leftToRightMod;
    public float LeftRight => leftToRightMod * 0.5f + 0.5f;
    private void OnEnable()
    {
        foreach (var ik in iks)
        {
            ik.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        foreach (var ik in iks)
        {
            ik.gameObject.SetActive(false);
        }
    }

    public void SetRightMovementDirectionModifier(float rightModifier)
    {
        leftToRightMod = rightModifier;
    }
}
