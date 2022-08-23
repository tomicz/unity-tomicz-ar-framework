using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARFoundation;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine.XR.Management;

namespace TOMICZ.AR
{
    public class DependencyInstaller
    {
        private static UniversalRendererData _universalRendererData;
        private static UniversalRenderPipelineAsset _universalRenderPipelineAsset;

        [MenuItem("Tomicz")]
        private static void Refresh()
        {
            AssetDatabase.Refresh();
            AssetDatabase.RefreshSettings();
        }

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
            InstallARFoundationDependencies();
            InstallARFoundationScene();

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

        private static void InstallARFoundationDependencies()
        {
#if UNITY_IOS
            AssigniOSXRPluginManagmentSettings();
#endif

#if UNITY_ANDROID
            AssignAndroidXRPluginManagmentSettings();
#endif
        }

        private static void AssigniOSXRPluginManagmentSettings()
        {
            PlayerSettings.iOS.targetOSVersionString = "12.0";
            PlayerSettings.iOS.cameraUsageDescription = "Required for augmented reality support.";

            XRGeneralSettingsPerBuildTarget buildTargetSettings = null;
            EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out buildTargetSettings);
            XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.iOS);

            XRPackageMetadataStore.AssignLoader(settings.Manager, "Unity.XR.ARKit.ARKitLoader", BuildTargetGroup.iOS);

            AssetDatabase.Refresh();
            AssetDatabase.RefreshSettings();
        }

        private static void AssignAndroidXRPluginManagmentSettings()
        {
            XRGeneralSettingsPerBuildTarget buildTargetSettings = null;
            EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out buildTargetSettings);
            XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Android);

            XRPackageMetadataStore.AssignLoader(settings.Manager, "Unity.XR.ARCore.ARCoreLoader", BuildTargetGroup.Android);
        }

        private static void InstallARFoundationScene()
        {
            Object.DestroyImmediate(Camera.main?.gameObject);
            EditorApplication.ExecuteMenuItem("GameObject/XR/AR Session");
            EditorApplication.ExecuteMenuItem("GameObject/XR/AR Session Origin");
            EditorSceneManager.SaveOpenScenes();
        }
    }
}