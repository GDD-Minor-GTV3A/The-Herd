using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Gameplay.Collectables 
{
    public abstract class Collectable : MonoBehaviour
    {
        protected Rigidbody rb;
        protected List<Collider> colliders;
        protected bool isCollectable = true;
        protected bool isCollected = false;


        protected void CheckCollider()
        {
            colliders = GetComponentsInChildren<Collider>().ToList();

            if (colliders == null || colliders.Count == 0)
            {
                List<MeshFilter> filters = GetComponentsInChildren<MeshFilter>().ToList();

                foreach (MeshFilter filter in filters)
                {
                    MeshCollider newCollider = gameObject.AddComponent<MeshCollider>();
                    newCollider.sharedMesh = filter.sharedMesh;
                    newCollider.convex = true;
                    colliders.Add(newCollider);
                }

                List<SkinnedMeshRenderer> skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();

                foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
                {
                    MeshCollider newCollider = gameObject.AddComponent<MeshCollider>();
                    newCollider.sharedMesh = renderer.sharedMesh;
                    newCollider.convex = true;
                    colliders.Add(newCollider);
                }
            }
        }

        protected void CheckRigidbody()
        {
            rb = GetComponentInChildren<Rigidbody>();

            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (isCollectable && !isCollected)
                    Collect();
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (isCollectable && !isCollected)
                    Collect();
            }
        }




        public abstract void Collect();

        public abstract void Spawn(Vector3 worldPosition, Vector3 forward);
    }
}