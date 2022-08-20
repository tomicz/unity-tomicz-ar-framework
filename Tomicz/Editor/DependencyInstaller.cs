using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARFoundation;

namespace TOMICZ.AR
{
    public class DependencyInstaller
    {
        private const string _tomiczFolderPath = "Assets/Tomicz/";
        private const string _assetsFolderPath = "Assets/";

        [MenuItem("Tomicz/AR Framework/Install")]
        private static void Install()
        {
            CreateFolder("Tomicz");
            CreatePiplineAsset("ARURPAsset");
        }

        private static void CreatePiplineAsset(string assetName)
        {
            UniversalRendererData universalRendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
            UniversalRenderPipelineAsset universalRendererPiplineAsset = UniversalRenderPipelineAsset.Create(universalRendererData);
            universalRendererData.postProcessData = GetDefaultPostProcessData();

            AssetDatabase.CreateAsset(universalRendererPiplineAsset, $"{_tomiczFolderPath + assetName}.asset");
            AssetDatabase.CreateAsset(universalRendererData, $"{_tomiczFolderPath + assetName + "Renderer"}.asset");

            ARBackgroundRendererFeature scriptableRendererFeature = ScriptableObject.CreateInstance<ARBackgroundRendererFeature>();
            scriptableRendererFeature.name = "ARBackgroundRendererFeature";
            universalRendererData.rendererFeatures.Add(scriptableRendererFeature);

            GraphicsSettings.defaultRenderPipeline = universalRendererPiplineAsset;
        }

        private static void CreateFolder(string path)
        {
            string persistentPath = _assetsFolderPath + path;

            if (!AssetDatabase.IsValidFolder(persistentPath))
            {
                Debug.Log($"Folder created succesfully at path: {persistentPath}");
                AssetDatabase.CreateFolder("Assets", "Tomicz");
            }
        }

        internal static PostProcessData GetDefaultPostProcessData()
        {
            var path = System.IO.Path.Combine(UniversalRenderPipelineAsset.packagePath, "Runtime/Data/PostProcessData.asset");
            return AssetDatabase.LoadAssetAtPath<PostProcessData>(path);
        }
    }
}