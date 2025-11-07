using CustomEditor.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.CameraSettings
{
    /// <summary>
    /// Config for camera.
    /// </summary>
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Configs/CameraConfig")]
    public class CameraConfig : ScriptableObject
    {
        [Header("Camera settings")]
        [SerializeField, Tooltip("Camera type.")] private CameraType cameraType;
        [SerializeField, Tooltip("Distance from camera to the player.")] private float defaultCameraDistance = 30;
        [SerializeField, ShowIf("ShowFOV"), Tooltip("FOV of perspective camera.")] private float FOV = 40;
        [SerializeField, Tooltip("Objects before this plane will not be rendered.")] private float nearClipPlane = 1;
        [SerializeField, Tooltip("Objects after this plane will not be rendered.")] private float farClipPlane = 1000;
        [SerializeField, Tooltip("Layers to render on main camera.")] private LayerMask renderLayers;
        [SerializeField, Tooltip("Position of camera.")] private Vector3 cameraAngles = new Vector3(30, 0 ,0);


        [Space]
        [Header("Dead Zone")]
        [SerializeField, Tooltip("Sizes of dead zone.")] private Vector2 size = new Vector2(0.1f, 0.15f);


        private bool ShowFOV => cameraType == CameraType.Perspective;


        /// <summary>
        /// Camera type.
        /// </summary>
        public CameraType Type => cameraType;
        /// <summary>
        /// Distance from camera to the player.
        /// </summary>
        public float DefaultCameraDistance => defaultCameraDistance;
        /// <summary>
        /// FOV of perspective camera.
        /// </summary>
        public float CameraFOV => FOV;
        /// <summary>
        /// Objects before this plane will not be rendered.
        /// </summary>
        public float NearClipPlane => nearClipPlane;
        /// <summary>
        /// Objects after this plane will not be rendered.
        /// </summary>
        public float FarClipPlane => farClipPlane;
        /// <summary>
        /// Layers to render on main camera.
        /// </summary>
        public LayerMask RenderLayers => renderLayers;

        /// <summary>
        /// Camera type.
        /// </summary>
        public Vector2 DeadZoneSize => size;
        /// <summary>
        /// Position of camera.
        /// </summary>
        public Vector3 CameraAngles => cameraAngles;



        /// <summary>
        /// Sizes of dead zone.
        /// </summary>
        public event UnityAction<CameraConfig> OnValueChanged;


        private void OnValidate()
        {
            if (nearClipPlane > farClipPlane)
                nearClipPlane = farClipPlane;

            if (cameraType == CameraType.Perspective && nearClipPlane <= 1)
                nearClipPlane = 1f;
                


            OnValueChanged?.Invoke(this);
        }
    }


    public enum CameraType
    {
        Orthographic,
        Perspective
    }
}