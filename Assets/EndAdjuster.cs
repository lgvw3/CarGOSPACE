using UnityEngine;

public class EndAdjuster : MonoBehaviour
{
    public Transform target;
    public Transform[] targetPositions; // Possible positions for the target

    public void MoveTarget(int episodeNumber, float cumulativeReward)
    {
        // Wait to change until we have learned a little about turns
        if (episodeNumber >= 10 && cumulativeReward > 7f)
        {
            target.position = targetPositions[Random.Range(0, targetPositions.Length + 1)].position;
        }
        else
        {
            // Default target placement for early episodes
            target.position = targetPositions[0].position;
        }
    }
}
