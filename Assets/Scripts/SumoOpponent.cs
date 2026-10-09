using UnityEngine;

namespace ThreeInOne
{
    /// <summary>Simple enemy movement for the sumo practice scene.</summary>
    public class SumoOpponent : MonoBehaviour
    {
        public Transform Target { get; set; }

        private Rigidbody body;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (Target == null)
            {
                return;
            }

            Vector3 direction = Target.position - transform.position;
            direction.y = 0f;
            body.AddForce(direction.normalized * 5f, ForceMode.Force);
        }
    }
}
