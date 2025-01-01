using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The object to follow
    public Vector3 offset = new Vector3(0, 0, -10); // Default offset (keep Z fixed for 2D)
    public float smoothSpeed = 0.125f; // Smoothing factor

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position (only X and Y, Z is fixed by the offset)
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, offset.z);

        // Smoothly interpolate to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Set the camera's position
        transform.position = smoothedPosition;
    }
}
