using UnityEngine;

public class FogRevealer : MonoBehaviour
{
    [SerializeField] private float _fov = 360f;
    [SerializeField] private float _viewDistance = 10f;
    [SerializeField] private int _rayCount = 50;
    [SerializeField] private Transform _fovOrigin;

    private GameObject _fovMeshObject;
    private Mesh _fovMesh;
    private LayerMask _obstaclesLayers;
    private float _startingAngle = 0;


    public void CreateFovMesh(Transform fogPlane, Material meshMaterial, LayerMask obstaclesLayers)
    {
        _fovMesh = new Mesh();

        _fovMeshObject = new GameObject();
        _fovMeshObject.name = "FovMesh";
        _fovMeshObject.transform.position = new Vector3(_fovOrigin.position.x, fogPlane.transform.position.y + 1, _fovOrigin.position.z);
        _fovMeshObject.transform.parent = fogPlane.transform.parent;
        _fovMeshObject.layer = LayerMask.NameToLayer("FogOfWar");
        _fovMeshObject.AddComponent<MeshFilter>().mesh = _fovMesh;
        _fovMeshObject.AddComponent<MeshRenderer>().material = meshMaterial;

        _obstaclesLayers = obstaclesLayers;

        UpdateMesh();
    }


    private void Update()
    {
        if (_fovMesh == null) return;
        _fovMeshObject.transform.position = new Vector3(_fovOrigin.position.x, _fovMeshObject.transform.position.y, _fovOrigin.position.z);
        Vector3 direction = _fovOrigin.forward;
        direction = direction.normalized;
        float result = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        result += 90f;

        if (result < 0) result += 360;

        _startingAngle = result - _fov / 2f;
        UpdateMesh();
    }



    private void UpdateMesh()
    {
        float angle = _startingAngle;
        float angleIncrease = _fov / _rayCount;
        Vector3 meshOrigin = Vector3.zero;

        Vector3[] vertices = new Vector3[_rayCount + 1 + 1];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[_rayCount * 3];

        vertices[0] = meshOrigin;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= _rayCount; i++)
        {
            float angleRad = angle * (Mathf.PI / 180f);
            Vector3 result = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
            Vector3 vertex;
            if (Physics.Raycast(_fovOrigin.position, result, out RaycastHit hit, _viewDistance, _obstaclesLayers))
            {
                Vector3 localHitPoint = _fovMeshObject.transform.InverseTransformPoint(hit.point + result * .5f);
                vertex = new Vector3(localHitPoint.x, meshOrigin.y, localHitPoint.z);
            }
            else
            {
                vertex = meshOrigin + result * _viewDistance;
            }

                vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }

            angle -= angleIncrease;
            vertexIndex++;
        }

        _fovMesh.vertices = vertices;
        _fovMesh.uv = uvs;
        _fovMesh.triangles = triangles;
    }
}
