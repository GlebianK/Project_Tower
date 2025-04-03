using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct HangRailMovementResult
{
    public RailSegment segment;
    public HangRail rail;
    public Vector3 position;
    public bool railStripEndReached;
}

public struct RailSegment
{
    public Vector3 leftCorner;
    public Vector3 rightCorner;

    public Vector3 forward => Vector3.Cross(right, Vector3.up);
    public Vector3 right => (rightCorner - leftCorner).normalized;
    public float length => (rightCorner - leftCorner).magnitude;
}

public class HangRail : MonoBehaviour
{
    [SerializeField] private HangRailConnector leftRailConnector;
    [SerializeField] private HangRailConnector rightRailConnector;
    [SerializeField] private Transform debugObject;

    private HangRail leftNeighbour;
    private HangRail rightNeighbour;

    private Vector3 fromLeftToRightDir => RightCorner - LeftCorner;

    public Vector3 forward { get; private set; }
    public Vector3 right => fromLeftToRightDir.normalized;
    public Vector3 LeftCorner => leftRailConnector.transform.position;
    public Vector3 RightCorner => rightRailConnector.transform.position;
    public float Length => fromLeftToRightDir.magnitude;

    private void Start()
    {
        leftNeighbour = leftRailConnector.GetNeighbour();
        rightNeighbour = rightRailConnector.GetNeighbour();

        forward = Vector3.Cross((RightCorner - LeftCorner).normalized, Vector3.up);
    }
    private void OnDrawGizmos()
    {
        if (leftRailConnector == null || rightRailConnector == null)
            return;

        HangRail leftNeighbour = leftRailConnector.GetNeighbour();
        HangRail rightNeighbour = rightRailConnector.GetNeighbour();

        Gizmos.color = Color.red;
        Gizmos.DrawLine(LeftCorner, RightCorner);

        Gizmos.color = Color.yellow;
        if (leftNeighbour != null)
        {
            Vector3[] leftConnectPoints = GetLineStripPointsBetween(leftNeighbour.RightCorner, LeftCorner, 100).ToArray();
            Gizmos.DrawLineStrip(leftConnectPoints, false);
        }

        if (rightNeighbour != null)
        {
            Vector3[] rightConnectPoints = GetLineStripPointsBetween(rightNeighbour.LeftCorner, RightCorner, 100).ToArray();
            Gizmos.DrawLineStrip(rightConnectPoints, false);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (leftRailConnector == null || rightRailConnector == null)
            return;

        Vector3 forward = Vector3.Cross((RightCorner - LeftCorner).normalized, Vector3.up);

        Gizmos.color = Color.blue;
        Vector3 lineStart = Vector3.Lerp(LeftCorner, RightCorner, 0.5f);
        Gizmos.DrawLine(lineStart, lineStart + forward * 0.4f);
    }

    private IEnumerable<Vector3> GetLineStripPointsBetween(Vector3 start, Vector3 end, int pointsCount)
    {
        for (int i = 0; i < pointsCount + 2; i++)
        {
            yield return Vector3.Lerp(start, end, i / (float)(pointsCount + 1));
        }
    }

    private Vector3 ProjectPositionOnRail(Vector3 point)
    {
        Vector3 currentLocalPositionOnRail = Vector3.Project(
            point - LeftCorner,
            right);

        return LeftCorner + currentLocalPositionOnRail;
    }

    private float GetOffset(Vector3 point) 
    {
        Vector3 snappedPos = ProjectPositionOnRail(point);

        float offsetAlongPath = (snappedPos - LeftCorner).magnitude;
        //to determinate direction - left or right
        offsetAlongPath *= Vector3.Dot((snappedPos - LeftCorner).normalized, right);

        return offsetAlongPath;
    }

    public Vector3 GetPositionOnRail(Vector3 point)
    {
        float offsetAlongPath = GetOffset(point);

        if (offsetAlongPath < 0)
        {
            float nextNeighbourOffset = offsetAlongPath;

            HangRail neighbour = leftNeighbour;
            if (neighbour != null)
            {
                point = neighbour.RightCorner + neighbour.right * nextNeighbourOffset;
                return neighbour.GetPositionOnRail(point);
            }
            else
            {
                return LeftCorner;
            }
        }

        if (offsetAlongPath > Length)
        {
            float nextNeighbourOffset = offsetAlongPath - Length;

            HangRail neighbour = rightNeighbour;
            if (neighbour != null)
            {
                point = neighbour.LeftCorner + neighbour.right * nextNeighbourOffset;
                return neighbour.GetPositionOnRail(point);
            }
            else
            {
                return RightCorner;
            }
        }

        return LeftCorner + right * offsetAlongPath;
    }

    public HangRail GetRailSegment(Vector3 point)
    {
        float offsetAlongPath = GetOffset(point);

        if (offsetAlongPath < 0)
        {
            float nextNeighbourOffset = offsetAlongPath;

            HangRail neighbour = leftNeighbour;
            if (neighbour != null)
            {
                point = neighbour.RightCorner + neighbour.right * nextNeighbourOffset;
                return neighbour.GetRailSegment(point);
            }
            else
            {
                return this;
            }
        }

        if (offsetAlongPath > Length)
        {
            float nextNeighbourOffset = offsetAlongPath - Length;

            HangRail neighbour = rightNeighbour;
            if (neighbour != null)
            {
                point = neighbour.LeftCorner + neighbour.right * nextNeighbourOffset;
                return neighbour.GetRailSegment(point);
            }
            else
            {
                return this;
            }
        }

        return this;
    }
}
