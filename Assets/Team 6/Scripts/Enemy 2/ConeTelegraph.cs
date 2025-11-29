using UnityEngine;

public class ConeTelegraph : MonoBehaviour
{
    [Header("Visuals")]
    [Tooltip("Base material to use for the cones (should be transparent).")]
    public Material baseMaterial;
    public Color outerColor = new Color(1f, 0f, 0f, 0.25f);     // big cone
    public Color innerColor = new Color(1f, 0.5f, 0f, 0.4f);    // inner high-damage area

    [Range(6, 64)]
    public int segments = 32;  // smoothness of the arc

    private Mesh outerMesh;
    private Mesh innerMesh;

    private MeshFilter outerFilter;
    private MeshFilter innerFilter;

    private MeshRenderer outerRenderer;
    private MeshRenderer innerRenderer;

    private void Awake()
    {
        // Create child object for the outer cone
        GameObject outerObj = new GameObject("OuterCone");
        outerObj.transform.SetParent(transform, false);
        outerObj.transform.localPosition = new Vector3(0f, 0f, 0f);        // on the ground
        outerObj.transform.localRotation = Quaternion.identity;
        outerObj.transform.localScale = Vector3.one;

        outerFilter = outerObj.AddComponent<MeshFilter>();
        outerRenderer = outerObj.AddComponent<MeshRenderer>();
        outerMesh = new Mesh();
        outerFilter.sharedMesh = outerMesh;

        // Create child object for the inner cone (slightly above to avoid z-fighting)
        GameObject innerObj = new GameObject("InnerCone");
        innerObj.transform.SetParent(transform, false);
        innerObj.transform.localPosition = new Vector3(0f, 0.1f, 0f); // 1 cm higher
        innerObj.transform.localRotation = Quaternion.identity;
        innerObj.transform.localScale = Vector3.one;

        innerFilter = innerObj.AddComponent<MeshFilter>();
        innerRenderer = innerObj.AddComponent<MeshRenderer>();
        innerMesh = new Mesh();
        innerFilter.sharedMesh = innerMesh;

        if (baseMaterial != null)
        {
            outerRenderer.material = new Material(baseMaterial);
            innerRenderer.material = new Material(baseMaterial);
            outerRenderer.material.color = outerColor;
            innerRenderer.material.color = innerColor;
        }

        Hide();
    }

    /// <summary>
    /// Build and show the two cones.
    /// outerRadius = full cone radius in WORLD units
    /// innerRadius = smaller inner cone radius (high damage / sheep kill) in WORLD units
    /// angleDeg   = cone angle in degrees.
    /// </summary>
    public void Show(float outerRadius, float innerRadius, float angleDeg)
    {
        if (outerRenderer == null || innerRenderer == null) return;

        // --- SCALE COMPENSATION ---
        // We want outerRadius / innerRadius to be in *world* units,
        // regardless of how this GameObject (or its parents) are scaled.

        float scaleX = transform.lossyScale.x;
        float scaleZ = transform.lossyScale.z;

#if UNITY_EDITOR
        if (Mathf.Abs(scaleX - scaleZ) > 0.001f)
        {
            Debug.LogWarning(
                $"[ConeTelegraph] Non-uniform X/Z scale on '{name}' " +
                $"(x={scaleX:F2}, z={scaleZ:F2}). Cone may look slightly squashed. " +
                "Try to keep X and Z scale the same."
            );
        }
#endif

        // Use average X/Z scale as an approximation
        float scale = (Mathf.Abs(scaleX) + Mathf.Abs(scaleZ)) * 0.5f;
        if (scale < 0.0001f) scale = 1f;

        float outerLocalRadius = outerRadius / scale;
        float innerLocalRadius = innerRadius / scale;

        BuildConeMesh(outerMesh, outerLocalRadius, angleDeg);
        BuildConeMesh(innerMesh, innerLocalRadius, angleDeg);

        outerRenderer.enabled = true;
        innerRenderer.enabled = true;
    }

    /// <summary>
    /// Hide the visual cones.
    /// </summary>
    public void Hide()
    {
        if (outerRenderer != null) outerRenderer.enabled = false;
        if (innerRenderer != null) innerRenderer.enabled = false;
    }

    private void BuildConeMesh(Mesh mesh, float radius, float angleDeg)
    {
        mesh.Clear();

        int seg = Mathf.Max(3, segments);
        int vertCount = seg + 2; // center + (seg+1) edge vertices

        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[seg * 3];

        // Center of the cone
        vertices[0] = Vector3.zero;

        float halfAngleRad = angleDeg * Mathf.Deg2Rad * 0.5f;
        float startAngle = -halfAngleRad;
        float totalAngle = halfAngleRad * 2f;
        float step = totalAngle / seg;

        // Build outer arc
        for (int i = 0; i <= seg; i++)
        {
            float a = startAngle + step * i;
            // Cone lies on XZ plane, pointing forward (local +Z)
            float x = Mathf.Sin(a) * radius;
            float z = Mathf.Cos(a) * radius;
            vertices[i + 1] = new Vector3(x, 0f, z);
        }

        // Triangle fan
        for (int i = 0; i < seg; i++)
        {
            int triIndex = i * 3;
            triangles[triIndex] = 0;
            triangles[triIndex + 1] = i + 1;
            triangles[triIndex + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
