using System.Reflection;

using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.FogOfWar 
{
    /// <summary>
    /// Creates additional collider of the object on a different layer, which is used during raycasts of revealers.
    /// </summary>
    public class ViewObstacle : MonoBehaviour
    {
        // for test, needs to be moved to bootstrap
        private void Start()
        {
            Initialize();
        }


        /// <summary>
        /// Initialization method
        /// </summary>
        public void Initialize()
        {
            Collider[] _objectColliders = GetComponentsInChildren<Collider>();

            if (_objectColliders.Length == 0)
            {
                Debug.LogError($"{name} does NOT contain colliders to make it a view obstacle!!!");
                return;
            }

            GameObject _obstacleObject = new GameObject("ViewObstacle");
            _obstacleObject.transform.parent = transform;
            _obstacleObject.layer = LayerMask.NameToLayer("ViewObstacles");
            _obstacleObject.transform.localPosition = Vector3.zero;
            _obstacleObject.transform.localScale = Vector3.one;
            _obstacleObject.transform.localRotation = Quaternion.identity;

            foreach (Collider _collider in _objectColliders)
            {
                System.Type _type = _collider.GetType();
                Collider _newCollider = _obstacleObject.AddComponent(_type) as Collider;

                BindingFlags _flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

                if (_collider is MeshCollider meshCollider)
                {
                    MeshCollider newMesh = (MeshCollider)_newCollider;
                    newMesh.sharedMesh = meshCollider.sharedMesh;
                    newMesh.convex = true;
                }

                _newCollider.isTrigger = true;
            }
        }
    }
}