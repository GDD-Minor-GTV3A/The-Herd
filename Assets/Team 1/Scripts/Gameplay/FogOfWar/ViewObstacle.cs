using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

namespace Gameplay.FogOfWar 
{
    public class ViewObstacle : MonoBehaviour
    {
        private void Start()
        {
            Initialize();
        }


        public void Initialize()
        {
            Collider[] objectColliders = GetComponentsInChildren<Collider>();

            if (objectColliders.Length == 0)
            {
                Debug.LogError($"{name} does NOT contain colliders to make it a view obstacle!!!");
                return;
            }

            GameObject obstacleObject = new GameObject("ViewObstacle");
            obstacleObject.transform.parent = transform;
            obstacleObject.layer = LayerMask.NameToLayer("ViewObstacles");
            obstacleObject.transform.localPosition = Vector3.zero;
            obstacleObject.transform.localScale = Vector3.one;
            obstacleObject.transform.localRotation = Quaternion.identity;

            foreach (Collider collider in objectColliders)
            {
                System.Type type = collider.GetType();
                Collider newCollider = obstacleObject.AddComponent(type) as Collider;

                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

                foreach (FieldInfo field in type.GetFields(flags))
                {
                    field.SetValue(newCollider, field.GetValue(collider));
                }

                collider.isTrigger = true;
            }
        }
    }
}