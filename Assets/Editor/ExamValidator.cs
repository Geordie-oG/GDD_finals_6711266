using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThreeInOne.Editor
{
    // Checks every exam grading item that can be verified without pressing Play.
    public static class ExamValidator
    {
        static readonly List<string> errors = new List<string>();

        [MenuItem("Three In One/Validate Exam Project")]
        public static bool Validate()
        {
            errors.Clear();

            // Build Settings: MainMenu first, then the three games
            string[] expected =
            {
                ExamSceneBuilder.MainMenuScene, ExamSceneBuilder.DrivingScene,
                ExamSceneBuilder.FlyingScene, ExamSceneBuilder.SumoScene
            };
            string[] actual = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            Check(actual.SequenceEqual(expected), "Build Settings scenes are not MainMenu, DrivingGame, FlyingGame, SumoGame");

            // Tags needed by the sumo game
            Check(InternalEditorUtility.tags.Contains("Enemy"), "Tag 'Enemy' is missing");
            Check(InternalEditorUtility.tags.Contains("Powerup"), "Tag 'Powerup' is missing");

            // Main Menu
            EditorSceneManager.OpenScene(ExamSceneBuilder.MainMenuScene);
            Check(Object.FindAnyObjectByType<MainMenu>() != null, "MainMenu: MainMenu component missing");
            CheckCommon("MainMenu");
            CheckButtons("MainMenu", "Mad Driver", "Fly Like a Bird", "I'm a Sumo and a Ball", "Exit");
            CheckText("MainMenu", "By " + ExamSceneBuilder.StudentName);

            // Game scenes
            CheckGame(ExamSceneBuilder.DrivingScene, "DrivingGame", typeof(VehicleController), typeof(FollowPlayer));
            CheckGame(ExamSceneBuilder.FlyingScene, "FlyingGame", typeof(PlayerControllerX), typeof(FollowPlayerX));
            CheckGame(ExamSceneBuilder.SumoScene, "SumoGame", typeof(PlayerController), typeof(SpawnManager));
            Check(GameObject.Find("Player") != null, "SumoGame: object 'Player' missing (Enemy.cs needs it)");
            Check(GameObject.Find("Focal Point") != null, "SumoGame: object 'Focal Point' missing");

            EditorSceneManager.OpenScene(ExamSceneBuilder.MainMenuScene);

            if (errors.Count == 0)
            {
                Debug.Log("[ThreeInOne] VALIDATION PASSED: all exam requirements found.");
                return true;
            }
            foreach (string error in errors)
            {
                Debug.LogError("[ThreeInOne] VALIDATION FAILED: " + error);
            }
            return false;
        }

        // Used from the command line: build, validate, then exit with 0 (pass) or 1 (fail).
        public static void BuildAndValidateFromCommandLine()
        {
            int code = 1;
            try
            {
                ExamSceneBuilder.BuildAll();
                code = Validate() ? 0 : 1;
            }
            catch (System.Exception exception)
            {
                Debug.LogError("[ThreeInOne] BUILD FAILED: " + exception);
            }
            EditorApplication.Exit(code);
        }

        static void CheckGame(string path, string label, System.Type playerType, System.Type otherType)
        {
            EditorSceneManager.OpenScene(path);
            CheckCommon(label);
            Check(Object.FindAnyObjectByType(playerType) != null, label + ": " + playerType.Name + " missing");
            Check(Object.FindAnyObjectByType(otherType) != null, label + ": " + otherType.Name + " missing");

            PauseMenu pauseMenu = Object.FindAnyObjectByType<PauseMenu>();
            Check(pauseMenu != null, label + ": PauseMenu missing");
            if (pauseMenu != null)
            {
                Check(pauseMenu.pausePanel != null, label + ": PauseMenu.pausePanel not assigned");
            }
            CheckText(label, "PAUSED");
            CheckButtons(label, "Resume", "Restart", "Back to Main Menu");
        }

        static void CheckCommon(string label)
        {
            Check(Object.FindAnyObjectByType<EventSystem>() != null, label + ": EventSystem missing");
            Check(Object.FindObjectsByType<Camera>(FindObjectsInactive.Exclude).Length > 0, label + ": no camera");
            foreach (MonoBehaviour behaviour in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include))
            {
                Check(behaviour != null, label + ": missing script reference");
            }
            foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include))
            {
                int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                Check(missing == 0, label + ": '" + go.name + "' has a missing script");
            }
        }

        static void CheckButtons(string label, params string[] labels)
        {
            Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include);
            foreach (string text in labels)
            {
                Button button = buttons.FirstOrDefault(b => b.GetComponentInChildren<Text>(true) != null && b.GetComponentInChildren<Text>(true).text == text);
                Check(button != null, label + ": button '" + text + "' missing");
                if (button != null)
                {
                    bool wired = button.onClick.GetPersistentEventCount() > 0 && button.onClick.GetPersistentTarget(0) != null
                        && !string.IsNullOrEmpty(button.onClick.GetPersistentMethodName(0));
                    Check(wired, label + ": button '" + text + "' has no onClick action");
                }
            }
        }

        static void CheckText(string label, string value)
        {
            bool found = Object.FindObjectsByType<Text>(FindObjectsInactive.Include).Any(t => t.text == value);
            Check(found, label + ": text '" + value + "' missing");
        }

        static void Check(bool condition, string message)
        {
            if (!condition) errors.Add(message);
        }
    }
}
