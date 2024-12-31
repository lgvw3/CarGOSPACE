using UnityEngine;

public class SimpleCar : MonoBehaviour
{
    public float speed = 5f; // Speed of the car
    public float turnSpeed = 5f; // How fast the car can turn
    private Rigidbody2D rb; // Reference to the Rigidbody2D component

    void Start()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from keyboard for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal != 0) 
        {
            // Rotate the car
            float angle = -moveHorizontal * turnSpeed * Time.deltaTime;
            transform.Rotate(0, 0, angle);

            // If rotating apply turn radius
            Vector2 turnRadiusMotion = transform.right * moveHorizontal * Time.deltaTime;
            //rb.MovePosition(rb.position + turnRadiusMotion);

            // Move the car based on input
            Vector2 movement = transform.up * moveVertical * speed * Time.deltaTime;
            rb.MovePosition(rb.position + movement + turnRadiusMotion);
        }
        else 
        {
            // Move the car based on input
            Vector2 movement = transform.up * moveVertical * speed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }
}