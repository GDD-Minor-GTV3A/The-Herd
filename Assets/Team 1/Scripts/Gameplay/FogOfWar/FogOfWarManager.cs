using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Shared;
using Core.Shared.Utilities;
using CustomEditor.Attributes;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Gameplay.FogOfWar 
{
    /// <summary>
    /// Main Fog Of War script. Creates all required objects, initialize revealers and update objects which are hidden if the fog.
    /// </summary>
    public class FogOfWarManager : MonoBehaviour
    {
        [SerializeField, Required, Tooltip("Player root object. Required for proper planes positioning.")] private Transform playerTransform;

        [Space]
        [Header("Configs")]
        [SerializeField, Required, Tooltip("General Fog Of War config.")] private FogOfWarConfig fogOfWarConfig;
        [SerializeField, Required, Tooltip("Data of level.")] private LevelData levelData;

        [Space]
        [Header("Debug")]
        [SerializeField, Tooltip("Shows the area where Fog Of War will be visible.")] private bool drawFogAreaGizmos = false;
        [SerializeField, Tooltip("Shows highest and lowest points of the map.")] private bool drawLevelBordersGizmos = false;
        [SerializeField, ShowIf("drawLevelBordersGizmos"), Tooltip("Size of planes that show highest and lowest points. Does NOT affect actual Fog Of War.")] private float levelBordersGizmosSize = 1000f;


        private float fogPlaneSize = 1f;
        private uint textureResolution = 100;
        private LayerMask obstaclesLayers;

        private Material fogProjectionMaterial;
        private Material revealerMaterial;
        private Material fogMaterial;

        private float mapHighestPoint = 0f;
        private float mapLowestPoint = 0f;

        private List<FogRevealer> revealers;
        private List<IHiddenObject> hiddenObjects;

        private ComputeShader hiddenComputeShader;
        private ComputeBuffer positionsBuffer;
        private ComputeBuffer visibilityBuffer;
        private Vector3[] positionsData;
        private uint[] visibilityData;

        private int shaderKernel;
        private uint threadsAmount;

        private RenderTexture fogTexture;
        private MeshRenderer fogProjectionPlane;
        private Camera renderCamera;
        private MeshRenderer fogEffectPlane;


        // for test, needs to be moved to bootstrap
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
            SetUpFogOverlayEffect();

            FindAllRevealers();
            FindAllHidden();

            UpdateBuffers();
            SetUpComputeShader();

            foreach (FogRevealer _revealer in revealers)
            {
                _revealer.Initialize(fogProjectionPlane.transform, revealerMaterial, obstaclesLayers);
            }

            fogOfWarConfig.OnValueChanged += UpdateValuesFromFogConfig;
            levelData.OnValueChanged += UpdateValuesFromLevelData;

            EventManager.AddListener<AddHiddenObjectEvent>(AddNewHiddenObject);
            EventManager.AddListener<RemoveHiddenObjectEvent>(RemoveHiddenObject);
        }


        private void AddNewHiddenObject(AddHiddenObjectEvent evt)
        {
            hiddenObjects.Add(evt.ObjectToAdd);
            UpdateBuffers();
            UpdateShaderValues();
        }

        private void RemoveHiddenObject(RemoveHiddenObjectEvent evt)
        {
            hiddenObjects.Remove(evt.ObjectToRemove);
            UpdateBuffers();
            UpdateShaderValues();
        }


        private void UpdateBuffers()
        {
            if (positionsBuffer != null)
                positionsBuffer?.Release();

            if (visibilityBuffer != null)
                visibilityBuffer?.Release();

            if (hiddenObjects.Count == 0) return;

            positionsBuffer = new ComputeBuffer(hiddenObjects.Count, sizeof(float) * 3);
            visibilityBuffer = new ComputeBuffer(hiddenObjects.Count, sizeof(uint));

            positionsData = new Vector3[hiddenObjects.Count];
            visibilityData = new uint[hiddenObjects.Count];
        }


        private void SetUpComputeShader()
        {
            shaderKernel = hiddenComputeShader.FindKernel("CSMain");
            hiddenComputeShader.GetKernelThreadGroupSizes(shaderKernel, out threadsAmount, out _, out _);

            UpdateShaderValues();
        }

        private void UpdateShaderValues()
        {
            if (hiddenObjects.Count == 0) return;
            hiddenComputeShader.SetBuffer(shaderKernel, "_ObjectPositions", positionsBuffer);
            hiddenComputeShader.SetBuffer(shaderKernel, "_ObjectVisibilities", visibilityBuffer);
            hiddenComputeShader.SetTexture(shaderKernel, "_FogTexture", fogTexture);
            hiddenComputeShader.SetFloat("_FogSize", fogPlaneSize);
        }

        private void SetUpFogOverlayEffect()
        {
            CreateFogEffectPlane();
            CreateFogEffectOverlayCamera();
        }

        private static void CreateFogEffectOverlayCamera()
        {
            Camera _mainCamera = Camera.main;

            Camera _newCamera = new GameObject("FogTextureRenderCamera").AddComponent<Camera>();

            _newCamera.transform.parent = _mainCamera.transform;
            _newCamera.transform.localPosition = Vector3.zero;
            _newCamera.transform.localRotation = Quaternion.identity;

            _newCamera.orthographic = _mainCamera.orthographic;
            _newCamera.orthographicSize = _mainCamera.orthographicSize;
            _newCamera.fieldOfView = _mainCamera.fieldOfView;
            _newCamera.nearClipPlane = _mainCamera.nearClipPlane;
            _newCamera.farClipPlane = _mainCamera.farClipPlane;

            _newCamera.cullingMask = LayerMask.GetMask("FogOfWarEffect");
            _newCamera.clearFlags = CameraClearFlags.SolidColor;
            _newCamera.backgroundColor = Color.black;

            _newCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            _mainCamera.GetUniversalAdditionalCameraData().cameraStack.Add(_newCamera);
        }

        private void CreateFogEffectPlane()
        {
            fogEffectPlane = GameObject.CreatePrimitive(PrimitiveType.Plane).GetComponent<MeshRenderer>();
            Destroy(fogEffectPlane.GetComponent<Collider>());
            fogEffectPlane.transform.parent = fogProjectionPlane.transform;
            fogEffectPlane.transform.localPosition = Vector3.zero;
            fogEffectPlane.transform.position = new Vector3(fogEffectPlane.transform.position.x,playerTransform.position.y, fogEffectPlane.transform.position.z);
            fogEffectPlane.transform.localRotation = Quaternion.Euler(0, 180, 0);
            fogEffectPlane.transform.localScale = Vector3.one;
            fogEffectPlane.gameObject.name = "FogPlaneEffect";
            fogEffectPlane.gameObject.layer = LayerMask.NameToLayer("FogOfWarEffect");
            fogEffectPlane.material = fogMaterial;
            fogMaterial.SetTexture("_MainTex", fogTexture);
        }

        private void CreateRenderCamera()
        {
            renderCamera = new GameObject("FogTextureRenderCamera").AddComponent<Camera>();
            renderCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
            renderCamera.transform.parent = fogProjectionPlane.transform;
            renderCamera.transform.localPosition = new Vector3(0, 5, 0);

            renderCamera.orthographic = true;
            renderCamera.orthographicSize = fogPlaneSize / 2;
            renderCamera.cullingMask = LayerMask.GetMask("FogOfWarProjection");
            renderCamera.clearFlags = CameraClearFlags.SolidColor;
            renderCamera.backgroundColor = Color.black;
            renderCamera.farClipPlane = 10;
            renderCamera.targetTexture = fogTexture;
        }

        private void CreateFogProjectionPlane()
        {
            fogProjectionPlane = GameObject.CreatePrimitive(PrimitiveType.Plane).GetComponent<MeshRenderer>();
            Destroy(fogProjectionPlane.GetComponent<Collider>());
            fogProjectionPlane.transform.parent = transform;
            fogProjectionPlane.transform.localRotation = Quaternion.Euler(0, 0, 0);
            fogProjectionPlane.transform.localPosition = new Vector3(0, mapHighestPoint + 1, 0);
            fogProjectionPlane.transform.localScale = new Vector3(fogPlaneSize / 10f, 1, fogPlaneSize / 10f);
            fogProjectionPlane.gameObject.name = "FogProjectionPlane";
            fogProjectionPlane.gameObject.layer = LayerMask.NameToLayer("FogOfWarProjection");
            fogProjectionPlane.material = fogProjectionMaterial;
        }

        private void CreateTexture()
        {
            fogTexture = new RenderTexture((int)textureResolution, (int)textureResolution, 0, RenderTextureFormat.ARGB32)
            {
                depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D32_SFloat_S8_UInt,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            fogTexture.Create();
        }


        private void FindAllRevealers()
        {
            revealers = FindObjectsByType<FogRevealer>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

            if (revealers.Count == 0)
                Debug.LogWarning("Fog Of War didn't find any revealers on the scene.");
        }

        private void FindAllHidden()
        {
            hiddenObjects = FindObjectsByType<HiddenInFog>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList<IHiddenObject>();
        }


        private void SetValuesFromConfigs()
        {
            fogPlaneSize = fogOfWarConfig.FogPlaneSize;
            textureResolution = fogOfWarConfig.TextureResolution;
            hiddenComputeShader = fogOfWarConfig.ComputeShader;
            obstaclesLayers = fogOfWarConfig.ObstaclesLayerMask;

            fogProjectionMaterial = fogOfWarConfig.FogProjectionMaterial;
            revealerMaterial = fogOfWarConfig.RevealerMaterial;
            fogMaterial = fogOfWarConfig.FogMaterial;

            mapHighestPoint = levelData.MapHighestPoint;
            mapLowestPoint = levelData.MapLowestPoint;
        }


        private void UpdateValuesFromFogConfig(FogOfWarConfig newConfig)
        {
            if (fogPlaneSize != fogOfWarConfig.FogPlaneSize)
            {
                fogPlaneSize = fogOfWarConfig.FogPlaneSize;
                fogProjectionPlane.transform.localScale = new Vector3(fogPlaneSize / 10f, 1, fogPlaneSize / 10f);

                renderCamera.orthographicSize = fogPlaneSize / 2;
                hiddenComputeShader.SetFloat("_FogSize", fogPlaneSize);
            }

            if (textureResolution != fogOfWarConfig.TextureResolution)
            {
                textureResolution = fogOfWarConfig.TextureResolution;

                fogTexture.Release();
                CreateTexture();

                renderCamera.targetTexture = fogTexture;
                fogMaterial.SetTexture("_MainTex", fogTexture);
                hiddenComputeShader.SetTexture(shaderKernel, "_FogTexture", fogTexture);
            }

            if (obstaclesLayers != fogOfWarConfig.ObstaclesLayerMask)
            {
                obstaclesLayers = fogOfWarConfig.ObstaclesLayerMask;

                foreach (FogRevealer _revealer in revealers)
                {
                    _revealer.UpdateObstaclesMask(obstaclesLayers);
                }
            }
            
            if (fogProjectionMaterial != fogOfWarConfig.FogProjectionMaterial)
            {
                fogProjectionMaterial = fogOfWarConfig.FogProjectionMaterial;

                fogProjectionPlane.material = fogProjectionMaterial;
            }

            if (revealerMaterial != fogOfWarConfig.RevealerMaterial)
            {
                revealerMaterial = fogOfWarConfig.RevealerMaterial;

                foreach (FogRevealer _revealer in revealers)
                {
                    _revealer.UpdateAllMaterials(revealerMaterial);
                }
            }

            if (fogMaterial != fogOfWarConfig.FogMaterial)
            {
                fogMaterial = fogOfWarConfig.FogMaterial;
                fogEffectPlane.material = fogMaterial;
                fogMaterial.SetTexture("_MainTex", fogTexture);
            }

            if (hiddenComputeShader != fogOfWarConfig.ComputeShader)
            {
                hiddenComputeShader = fogOfWarConfig.ComputeShader;
                SetUpComputeShader();
            }
        }

        private void UpdateValuesFromLevelData(LevelData newData)
        {
            if (mapHighestPoint != levelData.MapHighestPoint)
            {
                mapHighestPoint = levelData.MapHighestPoint;

                fogProjectionPlane.transform.localPosition = new Vector3(0, mapHighestPoint + 1, 0);

            }

            if (mapLowestPoint != levelData.MapLowestPoint)
            {
                mapLowestPoint = levelData.MapLowestPoint;
            }
        }


        private void UpdateHiddenObjectsVisibility()
        {
            if (hiddenObjects.Count == 0 || hiddenObjects == null) return; 

            for (int i = 0; i < positionsData.Length; i++)
            {
                positionsData[i] = hiddenObjects[i].GetPosition();
            }

            positionsBuffer.SetData(positionsData);

            hiddenComputeShader.SetVector("_PlayerPosition", playerTransform.position);
            //hiddenComputeShader.SetMatrix("_PlayerRotation", fogProjectionPlane.localToWorldMatrix);

            int _threadsToStart = Mathf.CeilToInt((float)hiddenObjects.Count / threadsAmount);

            hiddenComputeShader.Dispatch(shaderKernel, _threadsToStart, 1, 1);

            visibilityBuffer.GetData(visibilityData);

            for (int i = 0; i < hiddenObjects.Count; i++)
            {
                hiddenObjects[i].SetVisible(visibilityData[i] == 1);
            }
        }


        private void Update()
        {
            fogProjectionPlane.transform.position = new Vector3(playerTransform.position.x, fogProjectionPlane.transform.position.y, playerTransform.position.z);
            fogProjectionPlane.transform.rotation = playerTransform.rotation;
            fogEffectPlane.transform.position = new Vector3(fogEffectPlane.transform.position.x, playerTransform.position.y, fogEffectPlane.transform.position.z);

            UpdateHiddenObjectsVisibility();
        }


        private void OnDrawGizmos()
        {
            if (drawFogAreaGizmos)
            {
                if (fogOfWarConfig == null ||  playerTransform == null) return;

                Gizmos.color = new Color(0f, 0f, .7f, .5f);

                Gizmos.DrawCube(playerTransform.position + Vector3.up, new Vector3(fogOfWarConfig.FogPlaneSize, .001f, fogOfWarConfig.FogPlaneSize));
            }

            if (drawLevelBordersGizmos)
            {
                if (levelData == null) return;

                Gizmos.color = new Color(0f, .7f, 0f, .5f);

                Gizmos.DrawCube(transform.position + Vector3.up * levelData.MapHighestPoint, new Vector3(levelBordersGizmosSize, .001f, levelBordersGizmosSize));
                Gizmos.DrawCube(transform.position + Vector3.up * levelData.MapLowestPoint, new Vector3(levelBordersGizmosSize, .001f, levelBordersGizmosSize));
            }
        }


        private void OnDestroy()
        {
            fogOfWarConfig.OnValueChanged -= UpdateValuesFromFogConfig;
            levelData.OnValueChanged -= UpdateValuesFromLevelData;

            EventManager.RemoveListener<AddHiddenObjectEvent>(AddNewHiddenObject);
            EventManager.RemoveListener<RemoveHiddenObjectEvent>(RemoveHiddenObject);
        }
    }
}