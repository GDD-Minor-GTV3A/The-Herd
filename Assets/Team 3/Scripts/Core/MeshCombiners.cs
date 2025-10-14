using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class MeshCombiner : MonoBehaviour
{
    [ContextMenu("Combine Meshes")]
    void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Material> materials = new List<Material>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr == null) continue;

            Mesh mesh = mf.sharedMesh;

            for (int subMesh = 0; subMesh < mesh.subMeshCount; subMesh++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = mesh;
                ci.subMeshIndex = subMesh;
                ci.transform = transform.worldToLocalMatrix * mf.transform.localToWorldMatrix;
                combineInstances.Add(ci);

                if (subMesh < mr.sharedMaterials.Length)
                {
                    materials.Add(mr.sharedMaterials[subMesh]);
                }
            }
        }

        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(combineInstances.ToArray(), false, true);

        // Apply to this object
        MeshFilter newMF = gameObject.GetComponent<MeshFilter>();
        if (newMF == null) newMF = gameObject.AddComponent<MeshFilter>();
        newMF.sharedMesh = newMesh;

        MeshRenderer newMR = gameObject.GetComponent<MeshRenderer>();
        if (newMR == null) newMR = gameObject.AddComponent<MeshRenderer>();
        newMR.sharedMaterials = materials.ToArray();

#if UNITY_EDITOR
        // saves mesh as asset
        string path = "Assets/Combined_" + gameObject.name + ".asset";
        AssetDatabase.CreateAsset(newMesh, AssetDatabase.GenerateUniqueAssetPath(path));
        AssetDatabase.SaveAssets();
        Debug.Log("Saved combined mesh as asset: " + path);
#endif
    }
}
