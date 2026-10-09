using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ThreeInOne
{
    /// <summary>
    /// Base class shared by the three small games. It owns the common HUD and
    /// the Escape-key menu required by the exam paper.
    /// </summary>
    public abstract class GameSceneController : MonoBehaviour
    {
        private GameObject pauseOverlay;
        private Button resumeButton;
        private Text statusText;

        protected Transform player;
        protected bool IsPaused { get; private set; }

        protected abstract string GameTitle { get; }
        protected abstract string Instructions { get; }

        protected virtual void Start()
        {
            Time.timeScale = 1f;
            BuildGameplay();
            BuildHud();
            BuildPauseMenu();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }

            if (!IsPaused)
            {
                UpdateGameplay();
            }
        }

        protected abstract void BuildGameplay();

        protected virtual void UpdateGameplay()
        {
        }

        protected Camera CreateCamera(Vector3 position, Vector3 lookAt, Color skyColor)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.transform.position = position;
            cameraObject.transform.LookAt(lookAt);
            camera.backgroundColor = skyColor;
            camera.clearFlags = CameraClearFlags.SolidColor;
            return camera;
        }

        protected void CreateLight(Vector3 direction)
        {
            GameObject lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(direction);
        }

        protected GameObject CreatePrimitive(PrimitiveType type, string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject created = GameObject.CreatePrimitive(type);
            created.name = name;
            created.transform.position = position;
            created.transform.localScale = scale;

            Renderer renderer = created.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }

            return created;
        }

        protected void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        private void BuildHud()
        {
            Canvas canvas = UiFactory.CreateCanvas("Game HUD Canvas");
            GameObject header = UiFactory.CreatePanel(canvas.transform, "Header", new Color(0.02f, 0.04f, 0.09f, 0.72f));
            UiFactory.Stretch(header.GetComponent<RectTransform>(), new Vector2(0f, 0.89f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);

            Text title = UiFactory.CreateText(canvas.transform, "Game Title", GameTitle, 38, new Color(1f, 0.84f, 0.30f));
            UiFactory.Place(title.rectTransform, new Vector2(0f, 470f), new Vector2(900f, 54f));

            Text instructions = UiFactory.CreateText(canvas.transform, "Instructions", Instructions + "   |   Escape: Pause", 22, Color.white);
            UiFactory.Place(instructions.rectTransform, new Vector2(0f, 418f), new Vector2(1500f, 42f));

            statusText = UiFactory.CreateText(canvas.transform, "Status", "", 24, Color.white);
            UiFactory.Place(statusText.rectTransform, new Vector2(0f, -470f), new Vector2(1500f, 44f));
        }

        private void BuildPauseMenu()
        {
            Canvas canvas = UiFactory.CreateCanvas("In Game Menu Canvas");
            pauseOverlay = UiFactory.CreatePanel(canvas.transform, "Pause Overlay", new Color(0f, 0f, 0f, 0.75f));
            UiFactory.Stretch(pauseOverlay.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            GameObject card = UiFactory.CreatePanel(pauseOverlay.transform, "Paused Panel", new Color(0.08f, 0.16f, 0.28f, 1f));
            UiFactory.Place(card.GetComponent<RectTransform>(), Vector2.zero, new Vector2(700f, 520f));

            Text paused = UiFactory.CreateText(card.transform, "Paused Title", "PAUSED", 60, new Color(1f, 0.84f, 0.30f));
            UiFactory.Place(paused.rectTransform, new Vector2(0f, 155f), new Vector2(580f, 80f));

            resumeButton = CreatePauseButton(card.transform, "Resume Button", "Resume", 55f, ResumeGame);
            CreatePauseButton(card.transform, "Restart Button", "Restart", -45f, RestartGame);
            CreatePauseButton(card.transform, "Back Button", "Back to Main Menu", -145f, BackToMainMenu);

            pauseOverlay.SetActive(false);
        }

        private Button CreatePauseButton(Transform parent, string name, string label, float y, System.Action action)
        {
            Button button = UiFactory.CreateButton(parent, name, label, action);
            UiFactory.Place(button.GetComponent<RectTransform>(), new Vector2(0f, y), new Vector2(540f, 74f));
            return button;
        }

        private void TogglePause()
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                IsPaused = true;
                Time.timeScale = 0f;
                pauseOverlay.SetActive(true);
                EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
            }
        }

        public void ResumeGame()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            pauseOverlay.SetActive(false);
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void BackToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        protected virtual void OnDisable()
        {
            Time.timeScale = 1f;
        }
    }
}
