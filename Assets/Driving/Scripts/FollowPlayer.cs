using UnityEngine;

// Prototype 1: camera follows behind the vehicle.
public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset = new Vector3(0, 5, -7);

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
