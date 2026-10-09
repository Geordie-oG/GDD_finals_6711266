using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    private float powerupStrength = 15.0f;

    public float speed = 5.0f;
    public bool hasPowerup;
    public GameObject powerupIndicator;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        // Do not add forces while the game is paused
        if (Time.timeScale == 0f) return;

        float forwardInput = Input.GetAxis("Vertical");

        playerRb.AddForce(
            focalPoint.transform.forward * forwardInput * speed
        );

        powerupIndicator.transform.position =
            transform.position + new Vector3(0, -0.5f, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            Destroy(other.gameObject);

            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCountdownRoutine());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRigidbody =
                collision.gameObject.GetComponent<Rigidbody>();

            Vector3 awayFromPlayer =
                collision.gameObject.transform.position -
                transform.position;

            Debug.Log(
                "Player collided with " +
                collision.gameObject +
                " with powerup set to " +
                hasPowerup
            );

            enemyRigidbody.AddForce(
                awayFromPlayer * powerupStrength,
                ForceMode.Impulse
            );
        }
    }

    private IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);

        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }
}