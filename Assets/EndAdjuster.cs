using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using System;

public class EndAdjuster : MonoBehaviour
{
    public static EndAdjuster Instance; // Singleton instance for easy access
    public Transform target;
    public Transform[] targetPositions; // Possible positions for the target
    public AICar agent;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int OnEpisodeBegin()
    {
        int episodeNumber = agent.CompletedEpisodes; // Custom method in Agent
        float cumulativeReward = agent.GetCumulativeReward();

        int direction = 1; // 1 = left 0 = right
        // Wait to change until we have learned a little about turns
        if (episodeNumber >= 10 && cumulativeReward > 7f)
        {
            direction = UnityEngine.Random.Range(0, targetPositions.Length);
            Transform newTargetPosition = targetPositions[direction];
            target.position = newTargetPosition.position;
        }
        else
        {
            // Default target placement for early episodes
            target.position = targetPositions[0].position;
        }
        return direction;
    }
}
