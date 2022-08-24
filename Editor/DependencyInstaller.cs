using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARFoundation;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine.XR.Management;
using UnityEditor.Compilation;

namespace TOMICZ.AR
{
    [XRCustomLoaderUI("Unity.XR.ARKit.ARKitLoader", BuildTargetGroup.iOS)]
    public static class DependencyInstaller
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
            GenerateXRGeneralSettingsAndDependencies();
            //InstallARFoundationDependencies();
            InstallARFoundationScene();
            ChangeGraphicsRenderPipeline(_universalRenderPipelineAsset);
            //CompilationPipeline.RequestScriptCompilation();

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
            if(GraphicsSettings.defaultRenderPipeline == universalRenderPipelineAsset)
            {
                return;
            }

            GraphicsSettings.defaultRenderPipeline = universalRenderPipelineAsset;
            QualitySettings.renderPipeline = universalRenderPipelineAsset;
            AssetDatabase.RefreshSettings();
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

        private static void GenerateXRGeneralSettingsAndDependencies()
        {
            PlayerSettings.iOS.targetOSVersionString = "12.0";
            PlayerSettings.iOS.cameraUsageDescription = "Required for augmented reality support.";

            XRGeneralSettingsPerBuildTarget generalSettingsPerBuildTarget = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
            AssetDatabase.CreateAsset(generalSettingsPerBuildTarget, "Assets/XR/XRGeneralSettings.asset");
            generalSettingsPerBuildTarget.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.iOS);
            generalSettingsPerBuildTarget.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Android);

            var iosManager = generalSettingsPerBuildTarget.ManagerSettingsForBuildTarget(BuildTargetGroup.iOS);
            XRPackageMetadataStore.AssignLoader(iosManager, "Unity.XR.ARKit.ARKitLoader", BuildTargetGroup.iOS);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
            AssetDatabase.RefreshSettings();
        }

        private static void AssigniOSXRPluginManagmentSettings()
        {
            PlayerSettings.iOS.targetOSVersionString = "12.0";
            PlayerSettings.iOS.cameraUsageDescription = "Required for augmented reality support.";

            //string path = "Assets/XR/XRGeneralSettings.asset";
            //XRManagerSettings manager = AssetDatabase.LoadAssetAtPath<XRManagerSettings>(path);

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
            var session = Object.FindObjectOfType<ARSession>();
            var sessionOrigin = Object.FindObjectOfType<ARSessionOrigin>();

            if(session == null)
            {
                EditorApplication.ExecuteMenuItem("GameObject/XR/AR Session");
            }

            if(sessionOrigin == null)
            {
                Object.DestroyImmediate(Camera.main?.gameObject);
                EditorApplication.ExecuteMenuItem("GameObject/XR/AR Session Origin");
            }

            EditorSceneManager.SaveOpenScenes();
        }
    }
}