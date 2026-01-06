using System.Collections;
using Core.Events;
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
        [SerializeField, Required, Tooltip("Main camera.")] 
        private Camera mainCamera;

        [SerializeField, Required, Tooltip("Main CinemachineCamera.")] 
        private CinemachineCamera virtualCamera;

        [Space]
        [SerializeField, Required, Tooltip("Camera config.")] 
        private CameraConfig config;

        [SerializeField, Required, Tooltip("Player transform for camera to follow.")] 
        private Transform playerTransform;


        private CinemachinePositionComposer composer;
        private CinemachineBasicMultiChannelPerlin cameraNoise;
        private Coroutine shakeRoutine;
        private bool isShakingContinuously = false;


        /// <summary>
        /// Shakes camera for specific amount of time. 
        /// </summary>
        /// <param name="time">How long camera will be shaking.</param>
        public void ShakeCamera(float time)
        {
            if (isShakingContinuously) return;
            StopShakes();
            shakeRoutine = StartCoroutine(ShakeCameraRoutine(time));
        }

        /// <summary>
        /// Shakes camera Continuously 
        /// </summary>
        public void SetContinuousCameraShakes(bool active, float strength = 1.0f)
        {
            isShakingContinuously = active;

            if (active)
            {
                StopShakes();
                cameraNoise.AmplitudeGain = strength;
            }
            else
            {
                cameraNoise.AmplitudeGain = 0;
            }
        }

        private void StopShakes()
        {
            if (shakeRoutine != null)
            {
                StopCoroutine(shakeRoutine);
                cameraNoise.AmplitudeGain = 0;
            }
        }

        private void UpdateCameraSettings(CameraConfig newConfig)
        {
            if (config == null || playerTransform == null || mainCamera == null || virtualCamera == null) return;

            composer = virtualCamera.GetComponent<CinemachinePositionComposer>();
            if (composer == null) return;
            LensSettings _lensSettings = virtualCamera.Lens;

            if (config.Type == CameraType.Perspective)
            {
                _lensSettings.ModeOverride = LensSettings.OverrideModes.Perspective;
                _lensSettings.FieldOfView = config.CameraFOV;
            }
            else if (config.Type == CameraType.Orthographic)
            {
                _lensSettings.ModeOverride = LensSettings.OverrideModes.Orthographic;
                _lensSettings.OrthographicSize = config.DefaultCameraDistance;
            }
            composer.CameraDistance = config.DefaultCameraDistance;

            _lensSettings.NearClipPlane = config.NearClipPlane;
            _lensSettings.FarClipPlane = config.FarClipPlane;

            virtualCamera.Lens = _lensSettings;

            ScreenComposerSettings _composerSettings = composer.Composition;
            
            _composerSettings.DeadZone.Size = config.DeadZoneSize;
            composer.Composition = _composerSettings;


            mainCamera.cullingMask = config.RenderLayers;

            virtualCamera.Follow = playerTransform;
            virtualCamera.transform.rotation = Quaternion.Euler(config.CameraAngles.x, config.CameraAngles.y, config.CameraAngles.z);

            cameraNoise = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }


        private IEnumerator ShakeCameraRoutine(float time)
        {
            cameraNoise.AmplitudeGain = 1f;
            yield return new WaitForSecondsRealtime(time);
            cameraNoise.AmplitudeGain = 0;
        }


        private void CameraZoom(ZoomCameraEvent evt)
        {
            if (config.Type == CameraType.Orthographic)
            {
                LensSettings _lensSettings = virtualCamera.Lens;
                _lensSettings.OrthographicSize += evt.Value;
                virtualCamera.Lens = _lensSettings;
            }
            composer.CameraDistance += evt.Value;
        }


        private void OnEnable()
        {
            UpdateCameraSettings(config);

            EventManager.AddListener<ZoomCameraEvent>(CameraZoom);

            if (config != null)
                config.OnValueChanged += UpdateCameraSettings;
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<ZoomCameraEvent>(CameraZoom);


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