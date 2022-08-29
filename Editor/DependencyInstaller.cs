using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARFoundation;

namespace TOMICZ.AR
{
    public static class DependencyInstaller
    {
        private static UniversalRendererData _universalRendererData;
        private static UniversalRenderPipelineAsset _universalRenderPipelineAsset;

        [MenuItem("Tomicz/AR Framework/Install", false, 0)]
        private static void Install()
        {
            CreateFolder("Assets", "Rendering");
            CreateURPDependencies();
        }

        private static void CreateURPDependencies()
        {
            CreatePiplineAsset();
            CreateARRendererFeature(_universalRendererData);
            ChangeGraphicsRenderPipeline(_universalRenderPipelineAsset);

            Debug.Log("Successfully installed all TOMICZ AR Framework dependencies.");
        }

        private static void CreatePiplineAsset()
        {
            if(_universalRendererData == null)
            {
                _universalRendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(_universalRendererData, "Assets/Rendering/ARURPAssetRenderer.asset");
            }
            if (_universalRenderPipelineAsset == null)
            {
                _universalRenderPipelineAsset = UniversalRenderPipelineAsset.Create(_universalRendererData);
                AssetDatabase.CreateAsset(_universalRenderPipelineAsset, "Assets/Rendering/ARURPAsset.asset");
            }
            if (_universalRendererData.postProcessData == null)
            {
                _universalRendererData.postProcessData = GetDefaultPostProcessData();
            }

            Debug.Log("Installed Universal RP dependencies.");
        }

        private static void CreateARRendererFeature(UniversalRendererData universalRendererData)
        {
            ARBackgroundRendererFeature scriptableRendererFeature = ScriptableObject.CreateInstance<ARBackgroundRendererFeature>();
            scriptableRendererFeature.name = "ARBackgroundRendererFeature";
            AssetDatabase.CreateAsset(scriptableRendererFeature, "Assets/Rendering/ARBackgroundRendererFeature.asset");

            if (!universalRendererData.rendererFeatures.Contains(scriptableRendererFeature))
            {
                universalRendererData.rendererFeatures.Add(scriptableRendererFeature);
            }

            Debug.Log("Added ARBackgroundRendererFeature dependency to UniversalRendererData");
        }

        private static void ChangeGraphicsRenderPipeline(UniversalRenderPipelineAsset universalRenderPipelineAsset)
        {
            if(GraphicsSettings.defaultRenderPipeline == universalRenderPipelineAsset)
            {
                return;
            }

            GraphicsSettings.defaultRenderPipeline = universalRenderPipelineAsset;
            QualitySettings.renderPipeline = universalRenderPipelineAsset;
            AssetDatabase.RefreshSettings();

            Debug.Log("Changed default renderer pipeline asset in graphics settings.");
        }

        private static void CreateFolder(string path, string folderName)
        {
            if (!AssetDatabase.IsValidFolder(path + "/" + folderName))
            {
                AssetDatabase.CreateFolder(path, folderName);
                Debug.Log($"Created folder at: {path}. Folder name: {folderName}");
            }
        }

        internal static PostProcessData GetDefaultPostProcessData()
        {
            var path = System.IO.Path.Combine(UniversalRenderPipelineAsset.packagePath, "Runtime/Data/PostProcessData.asset");
            return AssetDatabase.LoadAssetAtPath<PostProcessData>(path);
        }
    }
}