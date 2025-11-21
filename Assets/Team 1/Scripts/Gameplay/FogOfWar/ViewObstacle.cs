using System.Reflection;
using UnityEngine;

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

                foreach (FieldInfo _field in _type.GetFields(_flags))
                {
                    _field.SetValue(_newCollider, _field.GetValue(_collider));
                }

                _collider.isTrigger = true;
            }
        }
    }
}