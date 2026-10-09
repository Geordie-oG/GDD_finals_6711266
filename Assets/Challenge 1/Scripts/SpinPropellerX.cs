using UnityEngine;

// Challenge 1 bonus: spin the plane's propeller.
public class SpinPropellerX : MonoBehaviour
{
    public float spinSpeed = 1000.0f;

    void Update()
    {
        transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
    }
}
