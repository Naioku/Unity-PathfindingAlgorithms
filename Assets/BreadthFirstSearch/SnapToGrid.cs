using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Snap();
    }

    private void Snap()
    {
        Vector3 gridSize = FindObjectOfType<GridManager>().Grid;
        var transformPosition = transform.position;
        var position = new Vector3(
            Mathf.Round(transformPosition.x / gridSize.x) * gridSize.x,
            Mathf.Round(transformPosition.y / gridSize.y) * gridSize.y,
            Mathf.Round(transformPosition.z / gridSize.z) * gridSize.z
        );

        transformPosition = position;
        transform.position = transformPosition;
    }
}
