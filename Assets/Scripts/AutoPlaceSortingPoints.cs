using UnityEngine;

public class AutoPlaceSortingPoints : MonoBehaviour
{
    public Renderer conveyorBeltRenderer;

    public Transform detectionPoint;
    public Transform sortingPoint;

    [Header("Offsets")]
    public float heightAboveBelt = 0.25f;
    public float detectionOffsetFromStart = 0.4f;
    public float sortingOffsetFromEnd = 1.5f;

    [Header("Direction")]
    public bool movementIsFromWallToCamera = true;

    void Start()
    {
        PlacePoints();
    }

    [ContextMenu("Place Points Now")]
    public void PlacePoints()
    {
        if (conveyorBeltRenderer == null || detectionPoint == null || sortingPoint == null)
        {
            Debug.LogError("Assign conveyorBeltRenderer, detectionPoint, and sortingPoint first.");
            return;
        }

        Bounds bounds = conveyorBeltRenderer.bounds;

        Vector3 center = bounds.center;
        Vector3 size = bounds.size;

        bool beltLongerOnZ = size.z >= size.x;

        Vector3 directionAxis = beltLongerOnZ ? Vector3.forward : Vector3.right;

        float halfLength = beltLongerOnZ ? bounds.extents.z : bounds.extents.x;

        Vector3 wallSide;
        Vector3 cameraSide;

        if (movementIsFromWallToCamera)
        {
            wallSide = center + directionAxis * halfLength;
            cameraSide = center - directionAxis * halfLength;
        }
        else
        {
            wallSide = center - directionAxis * halfLength;
            cameraSide = center + directionAxis * halfLength;
        }

        Vector3 detectionPos = Vector3.MoveTowards(wallSide, cameraSide, detectionOffsetFromStart);
        Vector3 sortingPos = Vector3.MoveTowards(cameraSide, wallSide, sortingOffsetFromEnd);

        detectionPos.y = bounds.max.y + heightAboveBelt;
        sortingPos.y = bounds.max.y + heightAboveBelt;

        detectionPoint.position = detectionPos;
        sortingPoint.position = sortingPos;

        Debug.Log("DetectionPoint Position = " + detectionPoint.position);
        Debug.Log("SortingPoint Position = " + sortingPoint.position);
        Debug.Log("Distance = " + Vector3.Distance(detectionPoint.position, sortingPoint.position));
    }
}