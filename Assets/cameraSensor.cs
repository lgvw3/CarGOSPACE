using UnityEngine;

public class CameraSensor : MonoBehaviour
{
    public float viewDistance = 2f; // How far the car can "see"
    public int viewAngle = 90; // The angle of the field of view
    public LayerMask viewLayer; // The layer on which the track tiles are (e.g., "Track")
    public Transform viewOrigin; // The point from which the view originates (front of the car)

    public int numberOfSegments = 10;

    void OnDrawGizmos()
    {
        if (viewOrigin != null)
        {
            // Draw the field of view for visualization
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(viewOrigin.position, viewDistance);
            Gizmos.color = Color.red;
            float cameraDefinition= 5f;

            float segmentAngle = viewAngle / (numberOfSegments - 1); // Adjust for the segments

            for (int i = 0; i < numberOfSegments; i++)
            {
                float angle = -viewAngle / 2 + i * segmentAngle;
                Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * viewOrigin.right;
                RaycastHit2D hit = Physics2D.Raycast(viewOrigin.position, rayDirection, viewDistance, viewLayer);
                if (hit.collider != null)
                {
                    // get a proportion of the viewspace that is identified as road vs obstacle

                    bool edgeDetected = false;
                    for (float j = .5f; j < cameraDefinition; j+=.5f)
                    {
                        Vector3 newMagnitude = rayDirection.normalized * j;
                        Vector2 newStart = viewOrigin.position + newMagnitude;
                        RaycastHit2D stillRoadHit = Physics2D.Raycast(newStart, rayDirection, viewDistance - newMagnitude.magnitude, viewLayer);
                        if (stillRoadHit.collider == null) 
                        {
                            edgeDetected = true;
                            Gizmos.DrawLine(viewOrigin.position, viewOrigin.position + rayDirection.normalized * (j - .5f));
                            break;
                        }
                    }

                    if (!edgeDetected)
                    {
                        Gizmos.DrawLine(viewOrigin.position, viewOrigin.position + rayDirection * viewDistance);
                    }

                }
            }
        }
    }

    public float[] DetectTilesInFOV()
    {
        float[] detectedTiles = new float[numberOfSegments]; // For each segment, we'll check if a tile is detected
        float segmentAngle = viewAngle / (numberOfSegments - 1); // Adjust for the segments
        float cameraDefinition= 5f;

        for (int i = 0; i < numberOfSegments; i++)
        {
            float angle = -viewAngle / 2 + i * segmentAngle;
            Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * viewOrigin.right;
            RaycastHit2D hit = Physics2D.Raycast(viewOrigin.position, rayDirection, viewDistance, viewLayer);

            if (hit.collider != null)
            {
                // get a proportion of the viewspace that is identified as road vs obstacle

                bool edgeDetected = false;
                for (float j = .5f; j < cameraDefinition; j+=.5f)
                {
                    Vector3 newMagnitude = rayDirection.normalized * j;
                    Vector2 newStart = viewOrigin.position + newMagnitude;
                    RaycastHit2D stillRoadHit = Physics2D.Raycast(newStart, rayDirection, viewDistance - newMagnitude.magnitude, viewLayer);
                    if (stillRoadHit.collider == null) 
                    {
                        detectedTiles[i] = Vector2.Distance(viewOrigin.position, newStart) / viewDistance;
                        edgeDetected = true;
                        break;
                    }
                }

                if (!edgeDetected) {
                    detectedTiles[i] = 1f;
                }

            }
            else
            {
                detectedTiles[i] = 0f;
            }
        }

        return detectedTiles;
    }
}
