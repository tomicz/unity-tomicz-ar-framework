using UnityEngine;
using UnityEditor;
using UnityEngine.XR.ARFoundation;
using UnityEditor.SceneManagement;

namespace TOMICZ.AR
{
    public class ARSceneInstaller
    {
        [MenuItem("Tomicz/AR Framework/Install AR Scene (Empty)", false, 1)]
        private static void InstallARFoundationScene()
        {
            var session = Object.FindObjectOfType<ARSession>();
            var sessionOrigin = Object.FindObjectOfType<ARSessionOrigin>();

            if (session == null)
            {
                EditorApplication.ExecuteMenuItem("GameObject/XR/AR Session");
            }

            if (sessionOrigin == null)
            {
                Object.DestroyImmediate(Camera.main?.gameObject);
                EditorApplication.ExecuteMenuItem("GameObject/XR/AR Session Origin");
            }

            EditorSceneManager.SaveOpenScenes();
        }
    }
}