using UnityEngine;

public class SimpleCar : MonoBehaviour
{
    public float speed = 5f; // Speed of the car
    public float turnSpeed = 5f; // How fast the car can turn
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    [SerializeField] private LayerMask trackLayer; // Assign this in inspector to your track's layer


    void Start()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }

    void Update()
    {
        // Get input from keyboard for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Move the car based on input
        Vector2 movement = transform.up * moveVertical * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

         // Turning
        float angle = -moveHorizontal * turnSpeed * Time.deltaTime;
        transform.Rotate(0, 0, angle);

        Collider2D trackCollider = Physics2D.OverlapCircle(transform.position, 0.1f, trackLayer);
        
        if (trackCollider != null)
        {
            Debug.Log("Crash");
            rb.position = new Vector3(0.001f, -0.482f, 0);
        }
    }
}