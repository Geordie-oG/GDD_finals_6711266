using UnityEngine;

// Prototype 1 (Mad Driver): drive the vehicle with W/S and steer with A/D.
public class VehicleController : MonoBehaviour
{
    public float speed = 20.0f;
    public float turnSpeed = 45.0f;

    private float horizontalInput;
    private float forwardInput;

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        // Move the vehicle forward / backward
        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
        // Turn the vehicle left / right
        transform.Rotate(Vector3.up, turnSpeed * horizontalInput * Time.deltaTime);
    }
}
