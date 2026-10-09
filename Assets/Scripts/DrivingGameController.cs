using UnityEngine;

namespace ThreeInOne
{
    public class DrivingGameController : GameSceneController
    {
        private Transform car;
        private Camera drivingCamera;

        protected override string GameTitle => "MAD DRIVER";
        protected override string Instructions => "W/S or Up/Down: drive     A/D or Left/Right: steer";

        protected override void BuildGameplay()
        {
            drivingCamera = CreateCamera(new Vector3(0f, 8f, -13f), new Vector3(0f, 0f, 6f), new Color(0.47f, 0.75f, 0.95f));
            CreateLight(new Vector3(50f, -30f, 0f));

            CreatePrimitive(PrimitiveType.Plane, "Grass", new Vector3(0f, 0f, 28f), new Vector3(3.5f, 1f, 12f), new Color(0.20f, 0.52f, 0.26f));
            CreatePrimitive(PrimitiveType.Cube, "Road", new Vector3(0f, 0.05f, 28f), new Vector3(10f, 0.12f, 100f), new Color(0.12f, 0.14f, 0.18f));

            for (int z = -5; z < 105; z += 8)
            {
                CreatePrimitive(PrimitiveType.Cube, "Road Marker", new Vector3(0f, 0.14f, z), new Vector3(0.3f, 0.04f, 3f), Color.white);
            }

            GameObject carObject = CreatePrimitive(PrimitiveType.Cube, "Player Car", new Vector3(0f, 0.65f, -6f), new Vector3(1.7f, 0.7f, 3.0f), new Color(0.90f, 0.16f, 0.13f));
            car = carObject.transform;
            player = car;

            CreateWheel(car, new Vector3(-0.95f, -0.43f, 0.90f));
            CreateWheel(car, new Vector3(0.95f, -0.43f, 0.90f));
            CreateWheel(car, new Vector3(-0.95f, -0.43f, -0.90f));
            CreateWheel(car, new Vector3(0.95f, -0.43f, -0.90f));

            CreateObstacle(new Vector3(-2.7f, 0.65f, 13f), new Color(0.12f, 0.35f, 0.85f));
            CreateObstacle(new Vector3(2.4f, 0.65f, 29f), new Color(1f, 0.70f, 0.10f));
            CreateObstacle(new Vector3(-1.5f, 0.65f, 48f), new Color(0.70f, 0.20f, 0.75f));
            SetStatus("Drive along the road and avoid the other cars.");
        }

        protected override void UpdateGameplay()
        {
            float throttle = Input.GetAxisRaw("Vertical");
            float steering = Input.GetAxisRaw("Horizontal");

            car.Translate(Vector3.forward * throttle * 11f * Time.deltaTime);
            car.Rotate(Vector3.up, steering * 95f * Time.deltaTime);
            car.position = new Vector3(Mathf.Clamp(car.position.x, -4.2f, 4.2f), 0.65f, Mathf.Clamp(car.position.z, -8f, 101f));

            Vector3 cameraTarget = car.position + new Vector3(0f, 7.5f, -12f);
            drivingCamera.transform.position = Vector3.Lerp(drivingCamera.transform.position, cameraTarget, 3.5f * Time.deltaTime);
            drivingCamera.transform.LookAt(car.position + car.forward * 7f);

            if (car.position.z >= 100f)
            {
                SetStatus("Finish line reached! Keep driving or use Escape to test the menu.");
            }
        }

        private void CreateWheel(Transform parent, Vector3 localPosition)
        {
            GameObject wheel = CreatePrimitive(PrimitiveType.Cylinder, "Wheel", parent.position, new Vector3(0.36f, 0.20f, 0.36f), Color.black);
            wheel.transform.SetParent(parent);
            wheel.transform.localPosition = localPosition;
            wheel.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }

        private void CreateObstacle(Vector3 position, Color color)
        {
            GameObject obstacle = CreatePrimitive(PrimitiveType.Cube, "Traffic Car", position, new Vector3(1.6f, 0.7f, 2.7f), color);
            obstacle.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
}
