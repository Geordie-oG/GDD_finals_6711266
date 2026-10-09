using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThreeInOne.Editor
{
    /// <summary>
    /// Creates the four deliberately small scenes. It is safe to run again if
    /// a scene was accidentally removed during a timed exam demonstration.
    /// </summary>
    public static class ProjectSetup
    {
        private const string SceneFolder = "Assets/Scenes";

        [MenuItem("Three In One/Create or Repair Exam Scenes")]
        public static void CreateOrRepairProject()
        {
            Directory.CreateDirectory(SceneFolder);
            CreateScene("MainMenu", typeof(MainMenuController));
            CreateScene("DrivingGame", typeof(DrivingGameController));
            CreateScene("FlyingGame", typeof(FlyingGameController));
            CreateScene("SumoGame", typeof(SumoGameController));

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(SceneFolder + "/MainMenu.unity", true),
                new EditorBuildSettingsScene(SceneFolder + "/DrivingGame.unity", true),
                new EditorBuildSettingsScene(SceneFolder + "/FlyingGame.unity", true),
                new EditorBuildSettingsScene(SceneFolder + "/SumoGame.unity", true)
            };

            AssetDatabase.SaveAssets();
            Debug.Log("Three In One scenes created and added to Build Settings.");
        }

        private static void CreateScene(string sceneName, Type controllerType)
        {
            string scenePath = SceneFolder + "/" + sceneName + ".unity";
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            GameObject controller = new GameObject(sceneName + " Controller");
            controller.AddComponent(controllerType);
            EditorSceneManager.SaveScene(scene, scenePath);
        }
    }
}
