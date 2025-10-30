using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ViewRecieve : MonoBehaviour
{
    [SerializeField]
    private List<MeshRenderer> meshesToHide;

    void Awake()
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

    }


    /*public void Order(bool outcome)
    {
        if (outcome)

        else

    }

    private void Appear()
    {
       foreach (MeshRenderer mesh in meshesToHide)
        {

        }
    }

    private void Dissapear()
    {

    }    */

}
