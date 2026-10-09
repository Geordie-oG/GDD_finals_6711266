using UnityEngine;

namespace ThreeInOne
{
    public class SumoGameController : GameSceneController
    {
        private Rigidbody playerBody;
        private int opponentsRemaining = 3;
        private const float ArenaRadius = 7.4f;

        protected override string GameTitle => "I'M A SUMO AND A BALL";
        protected override string Instructions => "W/A/S/D or arrow keys: move and push opponents from the ring";

        protected override void BuildGameplay()
        {
            CreateCamera(new Vector3(0f, 13f, -11f), Vector3.zero, new Color(0.44f, 0.70f, 0.92f));
            CreateLight(new Vector3(50f, -25f, 0f));

            CreatePrimitive(PrimitiveType.Plane, "Grass", Vector3.zero, new Vector3(3f, 1f, 3f), new Color(0.20f, 0.48f, 0.24f));
            CreatePrimitive(PrimitiveType.Cylinder, "Arena Base", new Vector3(0f, 0.12f, 0f), new Vector3(8.4f, 0.22f, 8.4f), new Color(0.45f, 0.25f, 0.13f));
            CreatePrimitive(PrimitiveType.Cylinder, "Sumo Ring", new Vector3(0f, 0.36f, 0f), new Vector3(7.7f, 0.10f, 7.7f), new Color(0.89f, 0.78f, 0.52f));

            GameObject playerObject = CreatePrimitive(PrimitiveType.Sphere, "Player Sumo Ball", new Vector3(0f, 1.15f, -2f), Vector3.one * 1.5f, new Color(0.90f, 0.16f, 0.13f));
            player = playerObject.transform;
            playerBody = playerObject.AddComponent<Rigidbody>();
            playerBody.mass = 2.2f;
            playerBody.linearDamping = 1.2f;
            playerBody.angularDamping = 2.5f;

            CreateOpponent("Blue Opponent", new Vector3(-3.4f, 1.0f, 2.8f), new Color(0.12f, 0.38f, 0.88f));
            CreateOpponent("Purple Opponent", new Vector3(3.5f, 1.0f, 2.6f), new Color(0.60f, 0.20f, 0.78f));
            CreateOpponent("Orange Opponent", new Vector3(0f, 1.0f, 4.7f), new Color(1.0f, 0.55f, 0.10f));
            SetStatus("Push the three opponent balls outside the ring. Opponents remaining: 3");
        }

        protected override void UpdateGameplay()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 force = new Vector3(horizontal, 0f, vertical).normalized * 22f;
            playerBody.AddForce(force, ForceMode.Force);

            if (FlatDistance(player.position) > ArenaRadius)
            {
                playerBody.linearVelocity = Vector3.zero;
                playerBody.angularVelocity = Vector3.zero;
                player.position = new Vector3(0f, 1.15f, -2f);
                SetStatus("You fell outside the ring. You were placed back in the arena.");
            }

            int activeOpponents = 0;
            foreach (SumoOpponent opponent in Object.FindObjectsByType<SumoOpponent>(FindObjectsSortMode.None))
            {
                if (opponent.gameObject.activeSelf)
                {
                    if (FlatDistance(opponent.transform.position) > ArenaRadius)
                    {
                        opponent.gameObject.SetActive(false);
                    }
                    else
                    {
                        activeOpponents++;
                    }
                }
            }

            if (activeOpponents != opponentsRemaining)
            {
                opponentsRemaining = activeOpponents;
                SetStatus(opponentsRemaining == 0
                    ? "You win! All opponents were pushed from the ring."
                    : "Push the opponent balls outside the ring. Opponents remaining: " + opponentsRemaining);
            }
        }

        private void CreateOpponent(string name, Vector3 position, Color color)
        {
            GameObject opponent = CreatePrimitive(PrimitiveType.Sphere, name, position, Vector3.one * 1.35f, color);
            Rigidbody body = opponent.AddComponent<Rigidbody>();
            body.mass = 1.1f;
            body.linearDamping = 1.3f;
            body.angularDamping = 2.2f;

            SumoOpponent behaviour = opponent.AddComponent<SumoOpponent>();
            behaviour.Target = player;
        }

        private static float FlatDistance(Vector3 position)
        {
            return new Vector2(position.x, position.z).magnitude;
        }
    }
}
