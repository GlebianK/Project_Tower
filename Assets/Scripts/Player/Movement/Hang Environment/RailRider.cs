using UnityEngine;


public class RailRider : MonoBehaviour
{
    [SerializeField] private HangRail rail;

    public HangRail Rail => rail;

    private void Start()
    {
        transform.position = rail.GetPositionOnRail(transform.position);
    }
    private void Update()
    {
        transform.position = rail.GetPositionOnRail(transform.position);
        rail = rail.GetRailSegment(transform.position);
    }
}
