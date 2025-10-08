using System.Collections.Generic;
using System.Linq;
using Core.Shared.Utilities;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Gameplay.FogOfWar 
{
    public class FogOfWarManager : MonoBehaviour
    {
        [SerializeField, Required, Tooltip("")] private Transform _playerTransform;

        [Space]
        [Header("Configs")]
        [SerializeField, Required, Tooltip("")] private FogOfWarConfig _fogOfWarConfig;
        [SerializeField, Required, Tooltip("")] private LevelData _levelData;

        [Space]
        [Header("Debug")]
        [SerializeField, Tooltip("")] private bool _drawFogAreaGizmos = false;
        [SerializeField, Tooltip("")] private bool _drawLevelBordersGizmos = false;
        [SerializeField, Tooltip("")] private float _levelBordersGizmosSize = 1000f;


        private float _fogPlaneSize = 1f;
        private uint _textureResolution = 100;
        private LayerMask _obstaclesLayers;

        private Material _fogProjectionMaterial;
        private Material _revealerMaterial;
        private Material _decalMaterial;
        private Material _fogMaterial;

        private float _mapHighestPoint = 0f;
        private float _mapLowestPoint = 0f;

        private List<FogRevealer> _revealers;
        private RenderTexture _fogTexture;
        private GameObject _fogProjPlane;
        private DecalProjector _decal;
        private Camera _renderCamera;


        private void Start()
        {
            Initialize();
        }


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize()
        {
            SetValuesFromConfigs();
            CreateFogProjectionPlane();
            CreateTexture();
            CreateRenderCamera();
            //CreateDecalProjector();
            SetUpFogOverlayEffect();

            FindAllRevealers();

            foreach (FogRevealer revealer in _revealers)
            {
                revealer.CreateFovMeshes(_fogProjPlane.transform, _revealerMaterial, _obstaclesLayers);
            }

            _fogOfWarConfig.OnValueChanged += UpdateValuesFromFogConfig;
            _levelData.OnValueChanged += UpdateValuesFromLevelData;
        }


        private void SetUpFogOverlayEffect()
        {
            CreateFogEffectPlane();
            CreateFogEffectOverlayCamera();
        }

        private static void CreateFogEffectOverlayCamera()
        {
            Camera mainCamera = Camera.main;

            Camera newCamera = new GameObject("FogTextureRenderCamera").AddComponent<Camera>();

            newCamera.transform.parent = mainCamera.transform;
            newCamera.transform.localPosition = Vector3.zero;
            newCamera.transform.localRotation = Quaternion.identity;

            newCamera.orthographic = mainCamera.orthographic;
            newCamera.orthographicSize = mainCamera.orthographicSize;
            newCamera.fieldOfView = mainCamera.fieldOfView;
            newCamera.nearClipPlane = mainCamera.nearClipPlane;
            newCamera.farClipPlane = mainCamera.farClipPlane;

            newCamera.cullingMask = LayerMask.GetMask("FoWEffect");
            newCamera.clearFlags = CameraClearFlags.SolidColor;
            newCamera.backgroundColor = Color.black;

            newCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            mainCamera.GetUniversalAdditionalCameraData().cameraStack.Add(newCamera);
        }

        private void CreateFogEffectPlane()
        {
            GameObject _fogPlaneeffcet = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(_fogPlaneeffcet.GetComponent<Collider>());
            _fogPlaneeffcet.transform.parent = _fogProjPlane.transform;
            _fogPlaneeffcet.transform.localPosition = Vector3.zero + Vector3.forward * _mapHighestPoint;
            _fogPlaneeffcet.transform.localRotation = Quaternion.identity;
            _fogPlaneeffcet.transform.localScale = Vector3.one;
            _fogPlaneeffcet.name = "FogPlaneEffect";
            _fogPlaneeffcet.layer = LayerMask.NameToLayer("FoWEffect");
            _fogPlaneeffcet.GetComponent<MeshRenderer>().material = _fogMaterial;
        }

        private void CreateDecalProjector()
        {
            _decal = new GameObject("DecalProjector").AddComponent<DecalProjector>();
            _decal.transform.parent = _fogProjPlane.transform;
            _decal.transform.localPosition = Vector3.zero;
            _decal.transform.localRotation = Quaternion.Euler(90, 0, 0);
            _decal.material = _decalMaterial;
            _decal.pivot = new Vector3(0, 0, ((_mapHighestPoint - _mapLowestPoint) / 2));
            _decal.size = new Vector3(_fogPlaneSize, _fogPlaneSize, _mapHighestPoint - _mapLowestPoint);
            _decal.fadeFactor = .85f;
            _decalMaterial.SetTexture("_MainTex", _fogTexture);
        }

        private void CreateRenderCamera()
        {
            _renderCamera = new GameObject("FogTextureRenderCamera").AddComponent<Camera>();
            _renderCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
            _renderCamera.transform.parent = _fogProjPlane.transform;
            _renderCamera.transform.localPosition = new Vector3(0, 5, 0);

            _renderCamera.orthographic = true;
            _renderCamera.orthographicSize = _fogPlaneSize / 2;
            _renderCamera.cullingMask = LayerMask.GetMask("FogOfWarProjection");
            _renderCamera.clearFlags = CameraClearFlags.SolidColor;
            _renderCamera.backgroundColor = Color.black;
            _renderCamera.farClipPlane = 10;
            _renderCamera.targetTexture = _fogTexture;
        }

        private void CreateFogProjectionPlane()
        {
            _fogProjPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Destroy(_fogProjPlane.GetComponent<Collider>());
            _fogProjPlane.transform.parent = transform;
            _fogProjPlane.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _fogProjPlane.transform.localPosition = new Vector3(0, _mapHighestPoint + 1, 0);
            _fogProjPlane.transform.localScale = new Vector3(_fogPlaneSize / 10f, 1, _fogPlaneSize / 10f);
            _fogProjPlane.name = "FogProjectionPlane";
            _fogProjPlane.layer = LayerMask.NameToLayer("FogOfWarProjection");
            _fogProjPlane.GetComponent<MeshRenderer>().material = _fogProjectionMaterial;
        }

        private void CreateTexture()
        {
            _fogTexture = new RenderTexture((int)_textureResolution, (int)_textureResolution, 0, RenderTextureFormat.ARGB32)
            {
                depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            _fogTexture.Create();
        }


        private void FindAllRevealers()
        {
            _revealers = FindObjectsByType<FogRevealer>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        }


        private void SetValuesFromConfigs()
        {
            _fogPlaneSize = _fogOfWarConfig.FogPlaneSize;
            _textureResolution = _fogOfWarConfig.TextureResolution;
            _obstaclesLayers = _fogOfWarConfig.ObstaclesLayerMask;

            _fogProjectionMaterial = _fogOfWarConfig.FogProjectionMaterial;
            _revealerMaterial = _fogOfWarConfig.RevealerMaterial;
            _decalMaterial = _fogOfWarConfig.DecalMaterial;
            _fogMaterial = _fogOfWarConfig.FogMaterial;

            _mapHighestPoint = _levelData.MapHighestPoint;
            _mapLowestPoint = _levelData.MapLowestPoint;
        }


        private void UpdateValuesFromFogConfig(FogOfWarConfig newConfig)
        {
            if (_fogPlaneSize != _fogOfWarConfig.FogPlaneSize)
            {
                _fogPlaneSize = _fogOfWarConfig.FogPlaneSize;
                _fogProjPlane.transform.localScale = new Vector3(_fogPlaneSize, _fogPlaneSize, 1);

                _renderCamera.orthographicSize = _fogPlaneSize / 2;

                if (_decal != null)
                {
                    _decal.pivot = new Vector3(0, 0, (_fogProjPlane.transform.localPosition.y / 2));
                    _decal.size = new Vector3(_fogPlaneSize, _fogPlaneSize, _mapHighestPoint - _mapLowestPoint);
                }
            }

            if (_textureResolution != _fogOfWarConfig.TextureResolution)
            {
                _textureResolution = _fogOfWarConfig.TextureResolution;

                _fogTexture.Release();
                CreateTexture();

                _renderCamera.targetTexture = _fogTexture;
                _decalMaterial.SetTexture("_MainTex", _fogTexture);
            }

            if (_obstaclesLayers != _fogOfWarConfig.ObstaclesLayerMask)
            {
                _obstaclesLayers = _fogOfWarConfig.ObstaclesLayerMask;

                foreach (FogRevealer revealer in _revealers)
                {
                    revealer.UpdateObstaclesMask(_obstaclesLayers);
                }
            }
            
            if (_fogProjectionMaterial != _fogOfWarConfig.FogProjectionMaterial)
            {
                _fogProjectionMaterial = _fogOfWarConfig.FogProjectionMaterial;

                _fogProjPlane.GetComponent<MeshRenderer>().material = _fogProjectionMaterial;
            }

            if (_revealerMaterial != _fogOfWarConfig.RevealerMaterial)
            {
                _revealerMaterial = _fogOfWarConfig.RevealerMaterial;

                foreach (FogRevealer revealer in _revealers)
                {
                    revealer.UpdateRevealerMaterial(_revealerMaterial);
                }
            }

            if (_decalMaterial != _fogOfWarConfig.DecalMaterial)
            {
                _decalMaterial = _fogOfWarConfig.DecalMaterial;

                if (_decal != null)
                    _decal.material = _decalMaterial;
            }
            
            //_fogMaterial = _fogOfWarConfig.FogMaterial;
        }

        private void UpdateValuesFromLevelData(LevelData newData)
        {
            if (_mapHighestPoint != _levelData.MapHighestPoint)
            {
                _mapHighestPoint = _levelData.MapHighestPoint;

                _fogProjPlane.transform.localPosition = new Vector3(0, _mapHighestPoint + 1, 0);

                if (_decal != null)
                    _decal.size = new Vector3(_fogPlaneSize, _fogPlaneSize, _mapHighestPoint - _mapLowestPoint);
            }

            if (_mapLowestPoint != _levelData.MapLowestPoint)
            {
                _mapLowestPoint = _levelData.MapLowestPoint;

                if (_decal != null)
                    _decal.size = new Vector3(_fogPlaneSize, _fogPlaneSize, _mapHighestPoint - _mapLowestPoint);
            }
        }


        private void Update()
        {
            _fogProjPlane.transform.position = new Vector3(_playerTransform.position.x, _fogProjPlane.transform.position.y, _playerTransform.position.z);
        }


        private void OnDrawGizmos()
        {
            if (_drawFogAreaGizmos)
            {
                if (_fogOfWarConfig == null ||  _playerTransform == null) return;

                Gizmos.color = new Color(0f, 0f, .7f, .5f);

                Gizmos.DrawCube(_playerTransform.position + Vector3.up, new Vector3(_fogOfWarConfig.FogPlaneSize, .001f, _fogOfWarConfig.FogPlaneSize));
            }

            if (_drawLevelBordersGizmos)
            {
                if (_levelData == null) return;

                Gizmos.color = new Color(0f, .7f, 0f, .5f);

                Gizmos.DrawCube(transform.position + Vector3.up * _levelData.MapHighestPoint, new Vector3(_levelBordersGizmosSize, .001f, _levelBordersGizmosSize));
                Gizmos.DrawCube(transform.position + Vector3.up * _levelData.MapLowestPoint, new Vector3(_levelBordersGizmosSize, .001f, _levelBordersGizmosSize));
            }
        }


        private void OnDestroy()
        {
            _fogOfWarConfig.OnValueChanged -= UpdateValuesFromFogConfig;
            _levelData.OnValueChanged -= UpdateValuesFromLevelData;
        }
    }
}