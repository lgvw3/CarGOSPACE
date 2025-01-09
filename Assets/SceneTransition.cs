using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using System;

public class SceneTransition : MonoBehaviour
{
    public string nextSceneName = "Curves"; // Name of the next scene
    public float performanceThreshold = 2f; // Threshold for performance to trigger scene change
    public int minimumSteps = 50000;
    public AICar agent;

    public void Awake()
    {
        Debug.Log("SceneTransition script started in Awake!");
        if (!agent) {
            Debug.Log("no agent");
        }
        else 
        {
            Debug.Log(agent.name);
        }
        //Academy.Instance.StatsRecorder.Add("Cumulative reward", agent.GetCumulativeReward());
    }

    private void Start()
    {
        Debug.Log("SceneTransition script started!");
        //Academy.Instance.StatsRecorder.Add("Cumulative reward", agent.GetCumulativeReward());
    }

    public void LogInfo()
    {
        // This prints to the terminal during training
        agent.GetCumulativeReward();
    }

    public void Update()
    {
        // Check if the agent has met the performance threshold in Scene 1
        Debug.Log("Cumulative Reward: " + agent.GetCumulativeReward().ToString() + ". Step count: " + agent.GetTotalStepsAcrossEpisodes());
        if (agent.GetCumulativeReward() >= performanceThreshold && agent.GetTotalStepsAcrossEpisodes() > minimumSteps)
        {
            Debug.Log("Threshold reached! Transitioning to next scene...");
            TransitionToNextScene();
        }
    }

    public void TransitionToNextScene()
    {
        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
