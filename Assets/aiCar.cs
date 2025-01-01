using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.Tilemaps;

public class AICar : Agent
{
    public float speed = 5f;
    public float turnSpeed = 25f;
    private Rigidbody2D rb;
    public Transform target;
    private Tilemap tilemap;
    
    public CameraSensor cameraSensor;

    private float lastDistance = 0f;
    private Vector3 lastPosition;


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
        sensor.AddObservation((target.position - transform.position).normalized); // Direction to target
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Actions: 0 for steering, 1 for acceleration
        float steer = actions.ContinuousActions[0];
        float accel = Mathf.Clamp(actions.ContinuousActions[1], 0f, 1f);

        Vector2 movement = transform.up * accel * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
        transform.Rotate(0, 0, -steer * turnSpeed * Time.deltaTime);

        // Reward System
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        AddReward(-0.001f); // Small penalty for each step to encourage efficiency
        if (tilemap == null) {
            tilemap = GameObject.FindGameObjectWithTag("TrackTilemap").GetComponent<Tilemap>();
        }
        Vector3Int gridPosition = tilemap.WorldToCell(transform.position);
        if (distanceToTarget < 1f) // If close to the target
        {
            AddReward(1.0f);
            Debug.Log("Successful Completion");
            EndEpisode(); // End the episode
            return;
        }
        else if (distanceToTarget < lastDistance)
        {
            //reward for progress
            AddReward(.01f);
        }
        lastDistance = distanceToTarget;
        if (!tilemap.cellBounds.Contains(gridPosition) || tilemap.GetTile(gridPosition) == null)
        {
            Debug.Log("Crash");
            AddReward(-1.0f); // Penalize going out of bounds
            EndEpisode();
        }
        // else if (steer > .2 || steer < -.2) // attempting smoothness
        // {
        //     AddReward(-.001f);
        // }
        // if (accel != 0) // bias towards action
        // {
        //     AddReward(.0005f);
        // }
        // float currentDistance = Vector3.Distance(transform.position, lastPosition);
        // float distanceDelta = currentDistance - lastDistance;
        
        // if (distanceDelta > 0) // Only reward forward movement
        // {
        //     float distanceReward = distanceDelta * .001f;
        //     AddReward(distanceReward);
        // }

    }

    public override void OnEpisodeBegin()
    {
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
        // // Reset car position to start point
        // transform.position = new Vector3(0.001f, -0.482f, 0); // Or wherever your start point is
        // transform.rotation = new Quaternion(0, 0, -90, 90);
        // rb.linearVelocity = Vector2.zero;
        // lastPosition = transform.position;
        // lastDistance = Vector2.Distance(transform.position, target.position);
    }
}
