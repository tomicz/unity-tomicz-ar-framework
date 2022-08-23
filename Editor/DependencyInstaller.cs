using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARFoundation;

namespace TOMICZ.AR
{
    public class DependencyInstaller
    {
        private static UniversalRendererData _universalRendererData;
        private static UniversalRenderPipelineAsset _universalRenderPipelineAsset;

        [MenuItem("Tomicz/AR Framework/Install")]
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

            Debug.Log("Successfully installed all dependencies.");
        }

        private static void CreatePiplineAsset()
        {
            _universalRendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
            _universalRenderPipelineAsset = UniversalRenderPipelineAsset.Create(_universalRendererData);

            if(_universalRendererData.postProcessData == null)
            {
                _universalRendererData.postProcessData = GetDefaultPostProcessData();
            }

            if (AssetExists(_universalRenderPipelineAsset))
            {
                AssetDatabase.CreateAsset(_universalRenderPipelineAsset, "Assets/Rendering/ARURPAsset.asset");
            }

            if (AssetExists(_universalRendererData))
            {
                AssetDatabase.CreateAsset(_universalRendererData, "Assets/Rendering/ARURPAssetRenderer.asset");
            }
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
        }

        private static void ChangeGraphicsRenderPipeline(UniversalRenderPipelineAsset universalRenderPipelineAsset)
        {
            if(GraphicsSettings.currentRenderPipeline != universalRenderPipelineAsset)
            {
                GraphicsSettings.defaultRenderPipeline = universalRenderPipelineAsset;
            }
        }

        private static void CreateFolder(string path, string folderName)
        {
            if (!AssetDatabase.IsValidFolder(path + "/" + folderName))
            {
                AssetDatabase.CreateFolder(path, folderName);
            }
        }

        internal static PostProcessData GetDefaultPostProcessData()
        {
            var path = System.IO.Path.Combine(UniversalRenderPipelineAsset.packagePath, "Runtime/Data/PostProcessData.asset");
            return AssetDatabase.LoadAssetAtPath<PostProcessData>(path);
        }

        private static bool AssetExists(Object asset)
        {
            if (AssetDatabase.GetAssetPath(asset) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}