using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameplay.FogOfWar
{
    public class FogRevealer : MonoBehaviour
    {
        [Serializable]
        private class Revealer
        {
            public FogRevealerConfig Config;

            [HideInInspector] public MeshRenderer Renderer;
            [HideInInspector] public float StartingAngle = 0;
            [HideInInspector] public Mesh Mesh;
        }


        [SerializeField, Tooltip("Data for every revealer for this object.")] private List<Revealer> revealers;
        [SerializeField, Tooltip("Origin point of revealer. If not assigned transform of object will be taken.")] private Transform origin;


        private LayerMask obstaclesLayers;


        /// <summary>
        /// Initializes the mesh for revealer.
        /// </summary>
        /// <param name="fogPlane">Transform of projection plan of the fog.</param>
        /// <param name="meshMaterial">Material for revealers.</param>
        /// <param name="obstaclesLayers">Layer mask of objects, which blocks the view.</param>
        public void CreateFovMeshes(Transform fogPlane, Material meshMaterial, LayerMask obstaclesLayers)
        {
            UpdateObstaclesMask(obstaclesLayers);

            for (int i = 0; i < revealers.Count; i++)
            {
                Mesh newMesh = new Mesh();

                GameObject newFovMeshObject = new GameObject();
                newFovMeshObject.name = "FovMesh";
                newFovMeshObject.transform.position = new Vector3(origin.position.x, fogPlane.transform.position.y + 1, origin.position.z);
                newFovMeshObject.transform.parent = fogPlane.transform.parent;
                newFovMeshObject.layer = LayerMask.NameToLayer("FogOfWarProjection");
                newFovMeshObject.AddComponent<MeshFilter>().mesh = newMesh;



                revealers[i].Renderer = newFovMeshObject.AddComponent<MeshRenderer>();
                revealers[i].Mesh = newMesh;
                UpdateMesh(i);
                UpdateRevealerMaterial(i, meshMaterial);
                StartCoroutine(UpdateMeshCor(revealers[i].Config.UpdateRate, i));
            }
        }


        private void Update()
        {
            for (int i = 0; i < revealers.Count; i++)
            {
                if (revealers[i].Renderer == null) continue;

                revealers[i].Renderer.transform.position = new Vector3(origin.position.x, revealers[i].Renderer.transform.position.y, origin.position.z);

                Vector3 _forward = origin.forward;
                _forward = _forward.normalized;
                float _startAngle = Mathf.Atan2(_forward.z, _forward.x) * Mathf.Rad2Deg;
                _startAngle += 150f;

                if (_startAngle < 0) _startAngle += 360;

                revealers[i].StartingAngle = _startAngle - revealers[i].Config.FOV / 2f;

                revealers[i].Renderer.material.SetVector("_RevealerCenter", revealers[i].Renderer.transform.position);
                revealers[i].Renderer.material.SetVector("_RevealerForward", origin.forward);
            }
        }


        private void UpdateMesh(int meshIndex)
        {
            float _fov = revealers[meshIndex].Config.FOV;
            float _viewDistance = revealers[meshIndex].Config.ViewDistance;
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
                    Vector3 _localHitPoint = revealers[meshIndex].Renderer.transform.InverseTransformPoint(hit.point);
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

            revealers[index].Renderer.material.SetFloat("_FOVAngle", revealers[index].Config.FOV * 0.5f * Mathf.Deg2Rad);
            revealers[index].Renderer.material.SetFloat("_ViewDistance", revealers[index].Config.ViewDistance);
            revealers[index].Renderer.material.SetFloat("_FadeWidth", 10f);
            revealers[index].Renderer.material.SetFloat("_EdgeFadeWidth", (revealers[index].Config.FOV == 360f) ? 0f : .5f);
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

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}