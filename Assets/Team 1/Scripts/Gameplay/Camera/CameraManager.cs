using Core.Shared.Utilities;
using Unity.Cinemachine;
using UnityEngine;

namespace Gameplay.CameraSettings 
{
    /// <summary>
    /// Manages camera's settings according to config.
    /// </summary>
    [ExecuteAlways]
    public class CameraManager : MonoBehaviour
    {
        [Header("Cameras")]
        [SerializeField, Required, Tooltip("Main camera.")] private Camera mainCamera;
        [SerializeField, Required, Tooltip("Main CinemachineCamera.")] private CinemachineCamera virtualCamera;

        [Space]
        [SerializeField, Required, Tooltip("Camera config.")] private CameraConfig config;
        [SerializeField, Required, Tooltip("Player transform for camera to follow.")] private Transform playerTransform;


        private void UpdateCameraSettings(CameraConfig newConfig)
        {
            if (config == null || playerTransform == null || mainCamera == null || virtualCamera == null) return;

            CinemachinePositionComposer _composer = virtualCamera.GetComponent<CinemachinePositionComposer>();
            if (_composer == null) return;
            LensSettings _lensSettings = virtualCamera.Lens;

            if (config.Type == CameraType.Perspective)
            {
                _lensSettings.ModeOverride = LensSettings.OverrideModes.Perspective;
                _lensSettings.FieldOfView = config.CameraFOV;
                _composer.CameraDistance = config.DefaultCameraDistance;
            }
            else if (config.Type == CameraType.Orthographic)
            {
                _lensSettings.ModeOverride = LensSettings.OverrideModes.Orthographic;
                _lensSettings.OrthographicSize = config.DefaultCameraDistance;
                _composer.CameraDistance = config.DefaultCameraDistance;
            }

            _lensSettings.NearClipPlane = config.NearClipPlane;
            _lensSettings.FarClipPlane = config.FarClipPlane;

            virtualCamera.Lens = _lensSettings;

            ScreenComposerSettings _composerSettings = _composer.Composition;
            
            _composerSettings.DeadZone.Size = config.DeadZoneSize;
            _composer.Composition = _composerSettings;


            mainCamera.cullingMask = config.RenderLayers;

            virtualCamera.Follow = playerTransform;
            virtualCamera.transform.rotation = Quaternion.Euler(config.CameraAngles.x, config.CameraAngles.y, config.CameraAngles.z);
        }


        private void OnEnable()
        {
            UpdateCameraSettings(config);

            if (config != null)
                config.OnValueChanged += UpdateCameraSettings;
        }

        private void OnDisable()
        {
            if (config != null)
                config.OnValueChanged -= UpdateCameraSettings;
        }


        private void OnValidate()
        {
            if (Application.isPlaying == false)
                UpdateCameraSettings(config);
        }
    }
}