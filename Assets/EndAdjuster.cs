using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class EndAdjuster : MonoBehaviour
{
    public GameObject target;
    public Transform[] targetPositions; // Possible positions for the target

    private int successfulEpisodesInARow = 0;

    public void MoveTarget(int episodeNumber, float cumulativeReward, bool reachedDestinationOnLastEpisode)
    {
        if (reachedDestinationOnLastEpisode) {
            successfulEpisodesInARow++;
        }
        else {
            successfulEpisodesInARow = 0;
        }
        int EPISODE_NUMBER = 0; // modify it for me when needed
        // Wait to change until we have learned a little about turns
        if (SceneManager.GetActiveScene().name == "Intersections")
        {
            Tilemap roadTiles = GameObject.FindGameObjectWithTag("TrackTilemap").GetComponent<Tilemap>();

        }
        Debug.Log($"{episodeNumber}, {cumulativeReward}");
        //step to help it learn to turn
        if (episodeNumber + EPISODE_NUMBER >= 950 && successfulEpisodesInARow > 5 && cumulativeReward > 1f && reachedDestinationOnLastEpisode) 
        {
            // the bigger step
            target.transform.position = targetPositions[Random.Range(2, targetPositions.Length)].position;
        }
        else if (episodeNumber + EPISODE_NUMBER >= 750 && successfulEpisodesInARow > 10 && cumulativeReward > 1f && reachedDestinationOnLastEpisode)
        {
            // the smaller step
            target.transform.position = targetPositions[Random.Range(0, 2)].position;
        }
        // else if (reachedDestinationOnLastEpisode)
        // {
        //     // Default target placement for early episodes
        //     target.transform.position = targetPositions[0].position;
        // }
    }
}
