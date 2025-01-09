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

    public void OnEpisodeBegin()
    {
        int episodeNumber = agent.CompletedEpisodes; // Custom method in Agent
        float cumulativeReward = agent.GetCumulativeReward();

        // Example logic: Adjust target placement only after 10 episodes
        if (episodeNumber >= 10 && cumulativeReward > 7f)
        {
            Transform newTargetPosition = targetPositions[UnityEngine.Random.Range(0, targetPositions.Length)];
            target.position = newTargetPosition.position;
        }
        else
        {
            // Default target placement for early episodes
            target.position = targetPositions[0].position;
        }
    }
}
