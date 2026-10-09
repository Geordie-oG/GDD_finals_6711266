using UnityEngine;

namespace ThreeInOne
{
    public class FlyingGameController : GameSceneController
    {
        private Transform bird;
        private Camera flyingCamera;
        private float verticalSpeed;
        private int gatesPassed;
        private float nextGateZ = 5f;

        protected override string GameTitle => "FLY LIKE A BIRD";
        protected override string Instructions => "Space: flap upward     A/D or Left/Right: steer";

        protected override void BuildGameplay()
        {
            flyingCamera = CreateCamera(new Vector3(0f, 5f, -13f), new Vector3(0f, 3f, 7f), new Color(0.25f, 0.66f, 0.95f));
            CreateLight(new Vector3(40f, -20f, 0f));

            CreatePrimitive(PrimitiveType.Plane, "Cloud Floor", new Vector3(0f, -1f, 30f), new Vector3(7f, 1f, 15f), new Color(0.72f, 0.87f, 0.95f));

            GameObject birdObject = CreatePrimitive(PrimitiveType.Sphere, "Player Bird", new Vector3(0f, 3f, -6f), new Vector3(1.3f, 0.85f, 1.6f), new Color(1f, 0.83f, 0.12f));
            bird = birdObject.transform;
            player = bird;

            CreateWing(new Vector3(-1.25f, 0f, 0f));
            CreateWing(new Vector3(1.25f, 0f, 0f));

            for (int i = 0; i < 7; i++)
            {
                CreateGate(new Vector3((i % 2 == 0 ? -2.5f : 2.5f), 3.4f, 5f + i * 11f));
                CreatePrimitive(PrimitiveType.Sphere, "Cloud", new Vector3((i % 3 - 1) * 6f, 1.5f + (i % 2), i * 13f), new Vector3(2.4f, 0.55f, 1.4f), Color.white);
            }

            SetStatus("Flap through the golden gates. Gates passed: 0");
        }

        protected override void UpdateGameplay()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalSpeed = 6.2f;
            }

            verticalSpeed += -7.5f * Time.deltaTime;
            float horizontal = Input.GetAxisRaw("Horizontal");
            bird.position += new Vector3(horizontal * 5f, verticalSpeed, 6.5f) * Time.deltaTime;
            bird.position = new Vector3(Mathf.Clamp(bird.position.x, -7.5f, 7.5f), Mathf.Clamp(bird.position.y, 0.6f, 9f), bird.position.z);

            if (bird.position.y <= 0.61f)
            {
                verticalSpeed = 0f;
            }

            bird.rotation = Quaternion.Euler(-verticalSpeed * 4f, 0f, -horizontal * 25f);
            flyingCamera.transform.position = Vector3.Lerp(flyingCamera.transform.position, bird.position + new Vector3(0f, 4f, -12f), 3f * Time.deltaTime);
            flyingCamera.transform.LookAt(bird.position + Vector3.forward * 7f);

            if (bird.position.z >= nextGateZ)
            {
                gatesPassed++;
                nextGateZ += 11f;
                SetStatus("Flap through the golden gates. Gates passed: " + gatesPassed);
            }

            if (bird.position.z > 78f)
            {
                bird.position = new Vector3(0f, 3f, -6f);
                verticalSpeed = 0f;
                gatesPassed = 0;
                nextGateZ = 5f;
                SetStatus("New flight started. Gates passed: 0");
            }
        }

        private void CreateWing(Vector3 localPosition)
        {
            GameObject wing = CreatePrimitive(PrimitiveType.Cube, "Bird Wing", bird.position, new Vector3(1.6f, 0.16f, 0.7f), Color.white);
            wing.transform.SetParent(bird);
            wing.transform.localPosition = localPosition;
        }

        private void CreateGate(Vector3 position)
        {
            GameObject left = CreatePrimitive(PrimitiveType.Cylinder, "Gate Left", position + Vector3.left * 1.4f, new Vector3(0.18f, 2.2f, 0.18f), new Color(1f, 0.68f, 0.05f));
            GameObject right = CreatePrimitive(PrimitiveType.Cylinder, "Gate Right", position + Vector3.right * 1.4f, new Vector3(0.18f, 2.2f, 0.18f), new Color(1f, 0.68f, 0.05f));
            CreatePrimitive(PrimitiveType.Cube, "Gate Top", position + Vector3.up * 2.1f, new Vector3(3f, 0.18f, 0.18f), new Color(1f, 0.68f, 0.05f));
            left.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            right.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
