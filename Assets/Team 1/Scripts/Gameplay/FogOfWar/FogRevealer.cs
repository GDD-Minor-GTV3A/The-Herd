using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameplay.FogOfWar
{
    /// <summary>
    /// Handles logic of creating and updating revealing mesh of the object for Fog Of War.
    /// </summary>
    public class FogRevealer : MonoBehaviour
    {
        [Serializable]
        protected class Revealer
        {
            public FogRevealerConfig Config;

            [HideInInspector] public MeshRenderer Renderer;
            [HideInInspector] public float StartingAngle = 0;
            [HideInInspector] public Mesh Mesh;
        }


        [SerializeField, Tooltip("Data for every revealer for this object.")] protected List<Revealer> revealers;
        [SerializeField, Tooltip("Origin point of revealer. If not assigned transform of object will be taken.")] protected Transform origin;


        private LayerMask obstaclesLayers;


        public virtual void Initialize(Transform fogPlane, Material meshMaterial, LayerMask obstaclesLayers)
        {
            CreateFovMeshes(fogPlane, meshMaterial, obstaclesLayers);
        }


        /// <summary>
        /// Initializes the mesh for revealer.
        /// </summary>
        /// <param name="fogPlane">Transform of projection plan of the fog.</param>
        /// <param name="meshMaterial">Material for revealers.</param>
        /// <param name="obstaclesLayers">Layer mask of objects, which blocks the view.</param>
        private void CreateFovMeshes(Transform fogPlane, Material meshMaterial, LayerMask obstaclesLayers)
        {
            if (origin == null)
                origin = transform;

            UpdateObstaclesMask(obstaclesLayers);

            for (int i = 0; i < revealers.Count; i++)
            {
                CreateNewMesh(i, fogPlane, meshMaterial);
            }
        }

        protected virtual void CreateNewMesh(int revealerIndex, Transform fogPlane, Material meshMaterial)
        {
            Mesh _newMesh = new Mesh();

            GameObject _newFovMeshObject = new GameObject();
            _newFovMeshObject.name = "FovMesh";
            _newFovMeshObject.transform.position = new Vector3(origin.position.x, fogPlane.transform.position.y + 2, origin.position.z);
            _newFovMeshObject.transform.parent = fogPlane.transform.parent;
            _newFovMeshObject.layer = LayerMask.NameToLayer("FogOfWarProjection");
            _newFovMeshObject.AddComponent<MeshFilter>().mesh = _newMesh;


            revealers[revealerIndex].Renderer = _newFovMeshObject.AddComponent<MeshRenderer>();
            revealers[revealerIndex].Mesh = _newMesh;
            UpdateMesh(revealerIndex);
            UpdateRevealerMaterial(revealerIndex, meshMaterial);

            if (!revealers[revealerIndex].Config.IsStatic)
                StartCoroutine(UpdateMeshCor(revealers[revealerIndex].Config.UpdateRate, revealerIndex));
        }


        private void Update()
        {
            for (int i = 0; i < revealers.Count; i++)
            {
                if (revealers[i].Renderer == null) continue;

                revealers[i].Renderer.transform.position = new Vector3(origin.position.x, revealers[i].Renderer.transform.position.y, origin.position.z);

                Vector3 _forward = origin.forward;
                float _startAngle = Mathf.Atan2(_forward.z, _forward.x) * Mathf.Rad2Deg;
                _startAngle += GetRevealerFOV(i);

                if (_startAngle < 0) _startAngle += 360;

                revealers[i].StartingAngle = _startAngle - GetRevealerFOV(i) / 2f;

                revealers[i].Renderer.material.SetVector("_RevealerCenter", origin.position);
                revealers[i].Renderer.material.SetVector("_RevealerForward", origin.forward);
            }
        }


        private void UpdateMesh(int meshIndex)
        {
            float _fov = GetRevealerFOV(meshIndex);
            float _viewDistance = GetRevealerDistance(meshIndex);
            int _rayCount = (int)revealers[meshIndex].Config.RayCount;

            float _angle = revealers[meshIndex].StartingAngle;
            float _angleIncrease = _fov / _rayCount;
            Vector3 _meshOrigin = Vector3.zero;

            Vector3[] _vertices = new Vector3[_rayCount + 1 + 1];
            int[] _triangles = new int[_rayCount * 3];

            _vertices[0] = _meshOrigin;

            int _vertexIndex = 1;
            int _triangleIndex = 0;

            for (int _v = 0; _v <= _rayCount; _v++)
            {
                float _angleRad = _angle * (Mathf.PI / 180f);
                Vector3 _rayDirection = new Vector3(Mathf.Cos(_angleRad), 0, Mathf.Sin(_angleRad));
                Vector3 _vertex;

                if (Physics.Raycast(origin.position, _rayDirection, out RaycastHit hit, _viewDistance, obstaclesLayers))
                {

                    Vector3 _localHitPoint = revealers[meshIndex].Renderer.transform.InverseTransformPoint(hit.point + _rayDirection * 3);
                    _vertex = new Vector3(_localHitPoint.x, _meshOrigin.y, _localHitPoint.z);
                }
                else
                {
                    _vertex = _meshOrigin + _rayDirection * _viewDistance;
                }

                _vertices[_vertexIndex] = _vertex;

                if (_v > 0)
                {
                    _triangles[_triangleIndex] = 0;
                    _triangles[_triangleIndex + 1] = _vertexIndex - 1;
                    _triangles[_triangleIndex + 2] = _vertexIndex;
                    _triangleIndex += 3;
                }

                _angle -= _angleIncrease;
                _vertexIndex++;
            }

            revealers[meshIndex].Mesh.vertices = _vertices;
            revealers[meshIndex].Mesh.triangles = _triangles;
        }


        protected virtual float GetRevealerFOV(int index)
        {
            return revealers[index].Config.FOV;
        }
        
        protected virtual float GetRevealerDistance(int index)
        {
            return revealers[index].Config.ViewDistance;
        }


        /// <summary>
        /// Updates obstacles layer mask for revealers.
        /// </summary>
        /// <param name="obstaclesLayers">New obstacle layer mask.</param>
        public void UpdateObstaclesMask(LayerMask obstaclesLayers)
        {
            this.obstaclesLayers = obstaclesLayers;
        }


        private void UpdateRevealerMaterial(int index, Material revealerMaterial)
        {
            if (revealers[index].Renderer == null) return;


            revealers[index].Renderer.material = revealerMaterial;

            revealers[index].Renderer.material.SetFloat("_FOVAngle", GetRevealerFOV(index) * 0.5f * Mathf.Deg2Rad);
            revealers[index].Renderer.material.SetFloat("_ViewDistance", GetRevealerDistance(index));
            revealers[index].Renderer.material.SetFloat("_FadeWidth", 10f);
            revealers[index].Renderer.material.SetFloat("_EdgeFadeWidth", (GetRevealerFOV(index) == 360f) ? 0f : (GetRevealerFOV(index) / 250));
        }


        /// <summary>
        /// Updates material on all meshes.
        /// </summary>
        /// <param name="revealerMaterial">New material.</param>
        public void UpdateAllMaterials(Material revealerMaterial)
        {
            for(int _i = 0; _i< revealers.Count; _i++)
            {
                UpdateRevealerMaterial(_i, revealerMaterial);
            }
        }


        private IEnumerator UpdateMeshCor(float updateTime, int meshIndex)
        {
            yield return null;
            while (true)
            {
                yield return new WaitForSeconds(updateTime);
                UpdateMesh(meshIndex);
            }
        }

        protected virtual void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}