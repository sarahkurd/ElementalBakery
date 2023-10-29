using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Player's Transform
    public float smoothSpeed = 0.125f;
    public float yOffset = 2.0f; // Offset on the y-axis

    void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(transform.position.x, target.position.y + yOffset, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}