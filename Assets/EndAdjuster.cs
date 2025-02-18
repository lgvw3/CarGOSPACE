using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class EndAdjuster : MonoBehaviour
{
    public GameObject target;
    public Transform[] targetPositions; // Possible positions for the target

    public void MoveTarget(int episodeNumber, float cumulativeReward, bool reachedDestinationOnLastEpisode)
    {
        // Wait to change until we have learned a little about turns
        if (SceneManager.GetActiveScene().name == "Intersections")
        {
            Tilemap roadTiles = GameObject.FindGameObjectWithTag("TrackTilemap").GetComponent<Tilemap>();

        }
        Debug.Log($"{episodeNumber}, {cumulativeReward}");
        if (episodeNumber >= 10 && cumulativeReward > 8f && reachedDestinationOnLastEpisode)
        {
            target.transform.position = targetPositions[Random.Range(0, targetPositions.Length)].position;
        }
        else
        {
            // Default target placement for early episodes
            target.transform.position = targetPositions[0].position;
        }
    }
}
