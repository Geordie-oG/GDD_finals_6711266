using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed = 100.0f;

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        transform.Rotate(
            Vector3.up,
            horizontalInput * rotationSpeed * Time.deltaTime
        );
    }
}