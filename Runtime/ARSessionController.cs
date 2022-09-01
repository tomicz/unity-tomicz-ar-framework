using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TOMICZ.AR
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class ARSessionController : MonoBehaviour
    {
        [Header("Select AR Experience Type")]

        [Tooltip("Enables ground scanning AR experience.")]
        [SerializeField] private bool _groundTracking = false;

        [Tooltip("Enable vertical scanning (walls).")]
        [SerializeField] private bool _wallTracking = false;

        [Header("Debugging")]
        [Tooltip("Show what is being scanned at runtime.")]
        [SerializeField] private bool _showDebuggerRuntime = false;

        private ARPlaneManager _planeManager;

        private void Awake()
        {
            _planeManager = GetComponent<ARPlaneManager>();
        }

        private void OnValidate()
        {
            _planeManager = GetComponent<ARPlaneManager>();

            EnableManager(_planeManager, _groundTracking);
            EnableManager(_planeManager, _wallTracking);
        }

        private void EnableManager(MonoBehaviour manager, bool value)
        {
            if (value)
            {
                if(manager == null)
                {
                    gameObject.AddComponent<ARPlaneManager>();
                }

                if(manager != null)
                {
                    manager.enabled = true;
                }
            }
            else
            {
                if(manager != null)
                {
                    manager.enabled = false;
                }
            }
        }

        private void EnableTracking(PlaneDetectionMode trackingMode, bool selectedTracking)
        {
            if (selectedTracking)
            {
                _planeManager.requestedDetectionMode = trackingMode;
            }
            else
            {
                _planeManager.requestedDetectionMode = PlaneDetectionMode.None;
            }
        }
    }
}