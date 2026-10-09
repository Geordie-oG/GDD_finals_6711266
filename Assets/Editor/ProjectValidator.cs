using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThreeInOne.Editor
{
    /// <summary>Batch-friendly static validation for the required exam features.</summary>
    public static class ProjectValidator
    {
        private static readonly string[] RequiredScenes =
        {
            "MainMenu",
            "DrivingGame",
            "FlyingGame",
            "SumoGame"
        };

        [MenuItem("Three In One/Validate Exam Project")]
        public static void Validate()
        {
            List<string> errors = new List<string>();
            ValidateScenes(errors);
            ValidateBuildSettings(errors);
            ValidateSourceLabels(errors);

            if (errors.Count > 0)
            {
                throw new Exception("PROJECT VALIDATION FAILED\n- " + string.Join("\n- ", errors));
            }

            Debug.Log("PROJECT VALIDATION PASSED: scenes, Build Settings, menu labels, and pause actions are present.");
        }

        private static void ValidateScenes(List<string> errors)
        {
            foreach (string sceneName in RequiredScenes)
            {
                string path = "Assets/Scenes/" + sceneName + ".unity";
                if (!File.Exists(path))
                {
                    errors.Add("Missing scene: " + path);
                    continue;
                }

                Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                if (scene.rootCount != 1)
                {
                    errors.Add(sceneName + " should contain one clear controller object.");
                }
            }
        }

        private static void ValidateBuildSettings(List<string> errors)
        {
            if (EditorBuildSettings.scenes.Length != RequiredScenes.Length)
            {
                errors.Add("Build Settings should contain exactly four game scenes.");
                return;
            }

            for (int index = 0; index < RequiredScenes.Length; index++)
            {
                string expectedPath = "Assets/Scenes/" + RequiredScenes[index] + ".unity";
                if (EditorBuildSettings.scenes[index].path != expectedPath || !EditorBuildSettings.scenes[index].enabled)
                {
                    errors.Add("Build Settings entry " + index + " should be " + expectedPath);
                }
            }
        }

        private static void ValidateSourceLabels(List<string> errors)
        {
            string mainMenuPath = "Assets/Scripts/MainMenuController.cs";
            string pausePath = "Assets/Scripts/GameSceneController.cs";
            string mainMenu = File.ReadAllText(mainMenuPath);
            string pauseMenu = File.ReadAllText(pausePath);

            Require(mainMenu, "Mad Driver", mainMenuPath, errors);
            Require(mainMenu, "Fly Like a Bird", mainMenuPath, errors);
            Require(mainMenu, "Sumo and a Ball", mainMenuPath, errors);
            Require(mainMenu, "Exit", mainMenuPath, errors);
            Require(pauseMenu, "KeyCode.Escape", pausePath, errors);
            Require(pauseMenu, "Resume", pausePath, errors);
            Require(pauseMenu, "Restart", pausePath, errors);
            Require(pauseMenu, "Back to Main Menu", pausePath, errors);
            Require(pauseMenu, "Time.timeScale = 0f", pausePath, errors);
        }

        private static void Require(string content, string requiredText, string fileName, List<string> errors)
        {
            if (!content.Contains(requiredText))
            {
                errors.Add(fileName + " does not contain required text: " + requiredText);
            }
        }
    }
}
