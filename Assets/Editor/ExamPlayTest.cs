using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ThreeInOne.Editor
{
    // Automated Play Mode smoke test of all 10 grading items.
    // Start with: touch Temp/run_exam_playtest (or menu Three In One > Run Play Test).
    // Screenshots go to Logs/PlayTest/, results are logged with [PlayTest].
    [InitializeOnLoad]
    public static class ExamPlayTest
    {
        const string Trigger = "Temp/run_exam_playtest";
        const string StepKey = "ExamPlayTest.Step";
        const string TimeKey = "ExamPlayTest.Time";
        const string FailKey = "ExamPlayTest.Failures";
        const string ShotFolder = "Logs/PlayTest";

        static readonly string[] Games = { "DrivingGame", "FlyingGame", "SumoGame" };
        static readonly string[] GameButtons = { "Mad Driver", "Fly Like a Bird", "I'm a Sumo and a Ball" };

        const int Steps = 11;

        static double nextCheck;

        static ExamPlayTest()
        {
            EditorApplication.update += Tick;
            Application.logMessageReceived += OnLog;
        }

        [MenuItem("Three In One/Run Play Test")]
        public static void Start()
        {
            Directory.CreateDirectory(ShotFolder);
            SessionState.SetInt(StepKey, 1);
            SessionState.SetInt(FailKey, 0);
            EditorSceneManager.OpenScene(ExamSceneBuilder.MainMenuScene);
            Debug.Log("[PlayTest] START");
            EditorApplication.EnterPlaymode();
        }

        static void OnLog(string message, string stack, LogType type)
        {
            if (SessionState.GetInt(StepKey, 0) > 0 && (type == LogType.Exception || type == LogType.Error)
                && !message.StartsWith("[PlayTest]"))
            {
                Fail("runtime error: " + message);
            }
        }

        static void Fail(string message)
        {
            SessionState.SetInt(FailKey, SessionState.GetInt(FailKey, 0) + 1);
            Debug.LogWarning("[PlayTest] FAIL " + message);
        }

        static void Pass(string message)
        {
            Debug.Log("[PlayTest] PASS " + message);
        }

        static void Expect(bool condition, string message)
        {
            if (condition) Pass(message); else Fail(message);
        }

        static void Shot(string name)
        {
            ScreenCapture.CaptureScreenshot(ShotFolder + "/" + name + ".png");
        }

        static void Click(string label)
        {
            Button button = Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude)
                .FirstOrDefault(b => b.GetComponentInChildren<Text>() != null && b.GetComponentInChildren<Text>().text == label);
            if (button == null)
            {
                PauseMenu pm = Object.FindAnyObjectByType<PauseMenu>();
                Fail("button '" + label + "' not found/visible in " + SceneManager.GetActiveScene().name
                    + " (panel active: " + (pm != null && pm.pausePanel.activeInHierarchy) + ", timeScale " + Time.timeScale
                    + ", pause menus: " + Object.FindObjectsByType<PauseMenu>(FindObjectsInactive.Include).Length + ")");
                return;
            }
            button.onClick.Invoke();
        }

        static PauseMenu Pause => Object.FindAnyObjectByType<PauseMenu>();

        static void Tick()
        {
            if (File.Exists(Trigger) && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                File.Delete(Trigger);
                Start();
                return;
            }

            int step = SessionState.GetInt(StepKey, 0);
            if (step == 0 || !EditorApplication.isPlaying) return;
            if (EditorApplication.timeSinceStartup < nextCheck) return;
            nextCheck = EditorApplication.timeSinceStartup + 1.5;   // wait between steps (real time)

            string scene = SceneManager.GetActiveScene().name;

            if (step == 1)
            {
                Expect(scene == "MainMenu", "1 Main Menu shown on start");
                Shot("01_MainMenu");
                SessionState.SetInt(StepKey, 2);
                return;
            }

            if (step >= 2 && step < 2 + Games.Length * Steps)
            {
                int g = (step - 2) / Steps;
                int s = (step - 2) % Steps;
                string game = Games[g];
                string prefix = "0" + (2 + g) + "_" + game;
                switch (s)
                {
                    case 0: Click(GameButtons[g]); break;
                    case 1:
                        Expect(scene == game, "Main Menu '" + GameButtons[g] + "' -> " + game);
                        Expect(Time.timeScale == 1f, game + " running (timeScale 1)");
                        break;
                    case 2: Shot(prefix); nextCheck = EditorApplication.timeSinceStartup + 4.0; break;
                    case 3: Pause.Pause(); break;   // same as pressing Escape
                    case 4:
                        Expect(Time.timeScale == 0f && Pause.pausePanel.activeSelf, game + " Escape pauses + PAUSED menu shown");
                        Shot(prefix + "_Paused");
                        nextCheck = EditorApplication.timeSinceStartup + 4.0;   // give the screenshot time to save
                        break;
                    case 5: Click("Resume"); break;
                    case 6:
                        Expect(Time.timeScale == 1f && !Pause.pausePanel.activeSelf, game + " Resume continues");
                        Pause.Pause();
                        break;
                    case 7: Click("Restart"); break;
                    case 8:
                        Expect(scene == game && Time.timeScale == 1f && !Pause.pausePanel.activeSelf, game + " Restart reloads the game");
                        Pause.Pause();
                        break;
                    case 9: Click("Back to Main Menu"); break;
                    case 10:
                        Expect(scene == "MainMenu" && Time.timeScale == 1f, game + " Back to Main Menu");
                        break;
                }
                SessionState.SetInt(StepKey, step + 1);
                return;
            }

            // Last: Exit button stops Play Mode
            int failures = SessionState.GetInt(FailKey, 0);
            SessionState.SetInt(StepKey, 0);
            Debug.Log(failures == 0
                ? "[PlayTest] ALL PASSED (Exit clicked next - Play Mode should stop)"
                : "[PlayTest] FINISHED WITH " + failures + " FAILURE(S)");
            Click("Exit");
        }
    }
}
