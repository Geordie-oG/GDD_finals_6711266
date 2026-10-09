using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThreeInOne.Editor
{
    // Lets the build/validate step be started from a terminal while the Editor is open:
    //   touch Temp/run_exam_build      -> Build All Scenes + Validate
    //   touch Temp/run_exam_validate   -> Validate only
    [InitializeOnLoad]
    public static class ExamCommandWatcher
    {
        const string BuildTrigger = "Temp/run_exam_build";
        const string ValidateTrigger = "Temp/run_exam_validate";
        static double nextCheck;

        static ExamCommandWatcher()
        {
            EditorApplication.update += Poll;
        }

        static void Poll()
        {
            if (EditorApplication.timeSinceStartup < nextCheck || EditorApplication.isPlayingOrWillChangePlaymode) return;
            nextCheck = EditorApplication.timeSinceStartup + 1.0;

            if (File.Exists(BuildTrigger))
            {
                File.Delete(BuildTrigger);
                Run(true);
            }
            else if (File.Exists(ValidateTrigger))
            {
                File.Delete(ValidateTrigger);
                Run(false);
            }
        }

        static void Run(bool build)
        {
            try
            {
                if (build) ExamSceneBuilder.BuildAll();
                ExamValidator.Validate();
            }
            catch (System.Exception exception)
            {
                Debug.LogError("[ThreeInOne] BUILD FAILED: " + exception);
            }
        }
    }
}
