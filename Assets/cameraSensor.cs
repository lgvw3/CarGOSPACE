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

            float segmentAngle = viewAngle / (numberOfSegments - 1); // Adjust for the segments

            for (int i = 0; i < numberOfSegments; i++)
            {
                float angle = -viewAngle / 2 + i * segmentAngle;
                Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * viewOrigin.right;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(viewOrigin.position, viewOrigin.position + rayDirection * viewDistance);
            }
        }
    }

    public float[] DetectTilesInFOV()
    {
        float[] detectedTiles = new float[numberOfSegments]; // For each segment, we'll check if a tile is detected
        float segmentAngle = viewAngle / (numberOfSegments - 1); // Adjust for the segments

        for (int i = 0; i < numberOfSegments; i++)
        {
            float angle = -viewAngle / 2 + i * segmentAngle;
            Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * viewOrigin.right;
            RaycastHit2D hit = Physics2D.Raycast(viewOrigin.position, rayDirection, viewDistance, viewLayer);

            if (hit.collider != null)
            {
                // get a proportion of the viewspace that is identified as road vs obstacle

                // TODO: I learned from the graph manager stuff that this isn't doing what I think it is lol
                // got to figure out how to do this for real. almost weird that it works
                detectedTiles[i] = hit.distance / viewDistance;
            }
            else
            {
                detectedTiles[i] = 0f;
            }
        }

        return detectedTiles;
    }
}
