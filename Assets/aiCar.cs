using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AICar : Agent
{
    public float speed = 5f;
    public float turnSpeed = 25f;
    private Rigidbody2D rb;
    public Transform target;
    private Tilemap tilemap;
    public LayerMask trackLayer; // Assign this in inspector to your track's layer
    
    public CameraSensor cameraSensor;
    public DynamicGraphGenerator2D navData;
    private int currentNavTarget = 0;

    private float lastDistance = 0f;
    private Vector3 lastPosition;

    private static int totalNumberOfStepsTaken = 0;

    public EndAdjuster endAdjuster;
    private float lastEpisodeReward = 0f;
    private bool reachedDestinationOnLastEpisode = false;

    public int GetTotalStepsAcrossEpisodes()
    {
        return totalNumberOfStepsTaken;
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {

        float[] tilesInView = cameraSensor.DetectTilesInFOV();
        foreach (float proportion in tilesInView)
        {
            sensor.AddObservation(proportion);
        }
        // Observations for the environment
        sensor.AddObservation(transform.rotation.z / 360f); // Car's rotation, steering wheel input simulation
        
        List<Vector2> path = navData.path;
        // see how close it is to the target gps equivalent
        // TODO: likely need to advance to next target based on some other trigger and do a new path finding
        float distanceToNextTarget = Vector2.Distance(path[currentNavTarget], transform.position);
        if (distanceToNextTarget < .25f && currentNavTarget < path.Count - 1) 
        {
            currentNavTarget += 1;
            distanceToNextTarget = Vector2.Distance(path[currentNavTarget], transform.position);

        }
        Debug.Log($"Current Nav Target Index: {currentNavTarget}. Distance to target: {distanceToNextTarget}");
        sensor.AddObservation(distanceToNextTarget);
        Vector2 directionToTarget = path[currentNavTarget] - new Vector2(transform.position.x, transform.position.y);
        sensor.AddObservation(directionToTarget.normalized); // Direction to nav target
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        totalNumberOfStepsTaken++;
        // Actions: 0 for steering, 1 for acceleration
        float steer = actions.ContinuousActions[0];
        float accel = Mathf.Clamp(actions.ContinuousActions[1], 0f, 1f);

        Vector2 movement = transform.up * accel * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
        transform.Rotate(0, 0, -steer * turnSpeed * Time.deltaTime);

        // Reward System
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        AddReward(-0.001f); // Small penalty for each step to encourage efficiency
        if (distanceToTarget < .25f) // If close to the target
        {
            AddReward(1.0f);
            Debug.Log("Successful Completion");
            lastEpisodeReward = GetCumulativeReward();
            reachedDestinationOnLastEpisode = true;
            EndEpisode(); // End the episode
        }
        else if (distanceToTarget < lastDistance)
        {
            //reward for progress
            AddReward(.005f);
        }
        lastDistance = distanceToTarget;
        
        if (tilemap == null) {
            tilemap = GameObject.FindGameObjectWithTag("TrackTilemap").GetComponent<Tilemap>();
        }
        Collider2D trackCollider = Physics2D.OverlapCircle(transform.position, 0.1f, trackLayer);
        if (trackCollider != null)
        {
            Debug.Log("Crash");
            AddReward(-1.0f); // Penalize going out of bounds
            lastEpisodeReward = GetCumulativeReward();
            reachedDestinationOnLastEpisode = false;
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        if (SceneManager.GetActiveScene().name == "90 Degree")
        {
            endAdjuster.MoveTarget(CompletedEpisodes, lastEpisodeReward, reachedDestinationOnLastEpisode);
        }
        else
        {
            Debug.Log($"{CompletedEpisodes}, {lastEpisodeReward}");
        }
        navData.AStarPathCreation();
        currentNavTarget = 1;
        if (navData.path.Count < 1) {
            currentNavTarget = 0;
        }
        // Define a small range for position and rotation randomness
        float positionRange = 0.01f; // Adjust as needed
        float rotationRange = 1f; // Degrees for rotation randomness
        // Base start position and rotation
        Vector3 basePosition = new Vector3(0.001f, -0.482f, 0);
        Quaternion baseRotation = new Quaternion(0, 0, -90, 90);

        // Randomize position slightly
        float xOffset = Random.Range(-positionRange, positionRange);
        float yOffset = Random.Range(-positionRange, positionRange);
        transform.position = basePosition + new Vector3(xOffset, yOffset, 0);

        // Randomize rotation slightly
        float rotationOffset = Random.Range(-rotationRange, rotationRange);
        transform.rotation = baseRotation * Quaternion.Euler(0, 0, rotationOffset);

        // Reset other parameters
        rb.linearVelocity = Vector2.zero;
        lastPosition = transform.position;
    }
}
