using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ThreeInOne
{
    public class MainMenuController : MonoBehaviour
    {
        private Text editorExitMessage;

        private void Start()
        {
            Time.timeScale = 1f;
            BuildMenu();
        }

        private void BuildMenu()
        {
            GameObject cameraObject = new GameObject("Main Menu Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.04f, 0.08f, 0.16f);

            Canvas canvas = UiFactory.CreateCanvas("Main Menu Canvas");

            GameObject background = UiFactory.CreatePanel(canvas.transform, "Background", new Color(0.04f, 0.08f, 0.16f, 1f));
            UiFactory.Stretch(background.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            Text title = UiFactory.CreateText(canvas.transform, "Title", "THREE IN ONE", 78, new Color(1f, 0.82f, 0.28f));
            UiFactory.Place(title.rectTransform, new Vector2(0f, 310f), new Vector2(1100f, 120f));

            Text subtitle = UiFactory.CreateText(canvas.transform, "Subtitle", "GAME COLLECTION", 32, Color.white);
            UiFactory.Place(subtitle.rectTransform, new Vector2(0f, 235f), new Vector2(900f, 60f));

            Text byline = UiFactory.CreateText(canvas.transform, "Byline", "By Student", 24, new Color(0.75f, 0.84f, 0.96f));
            UiFactory.Place(byline.rectTransform, new Vector2(0f, 185f), new Vector2(700f, 50f));

            Button drivingButton = CreateMenuButton(canvas.transform, "Driving Button", "Mad Driver", 95f, () => LoadGame("DrivingGame"));
            CreateMenuButton(canvas.transform, "Flying Button", "Fly Like a Bird", 5f, () => LoadGame("FlyingGame"));
            CreateMenuButton(canvas.transform, "Sumo Button", "I'm a Sumo and a Ball", -85f, () => LoadGame("SumoGame"));
            CreateMenuButton(canvas.transform, "Exit Button", "Exit", -175f, ExitGame);

            EventSystem.current.firstSelectedGameObject = drivingButton.gameObject;
            EventSystem.current.SetSelectedGameObject(drivingButton.gameObject);

            Text tip = UiFactory.CreateText(canvas.transform, "Tip", "Choose a game. Press Escape during gameplay to open the pause menu.", 22, new Color(0.75f, 0.84f, 0.96f));
            UiFactory.Place(tip.rectTransform, new Vector2(0f, -315f), new Vector2(1150f, 50f));

            editorExitMessage = UiFactory.CreateText(canvas.transform, "Exit Message", "Exit requested. Application.Quit works in a built game.", 24, new Color(1f, 0.82f, 0.28f));
            UiFactory.Place(editorExitMessage.rectTransform, new Vector2(0f, -370f), new Vector2(1200f, 45f));
            editorExitMessage.gameObject.SetActive(false);
        }

        private Button CreateMenuButton(Transform parent, string name, string label, float y, System.Action action)
        {
            Button button = UiFactory.CreateButton(parent, name, label, action);
            UiFactory.Place(button.GetComponent<RectTransform>(), new Vector2(0f, y), new Vector2(610f, 70f));
            return button;
        }

        private void LoadGame(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        private void ExitGame()
        {
            Debug.Log("Exit was selected from the Main Menu.");
            editorExitMessage.gameObject.SetActive(true);
            Application.Quit();
        }
    }
}
