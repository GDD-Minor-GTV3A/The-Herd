using Core.AI.Sheep;
using UnityEngine;

namespace Game.Scripts.Gameplay.Projectiles.Amalgamation
{
    public class BulletScript : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _speed = 15f;
        [SerializeField] private float _lifeTime = 3f;

        [Header("Damage")]
        [SerializeField] private float _damage = 20f;

        [Header("Filtering")]
        [Tooltip("Set this to the Sheep layer in the prefab. If left to Nothing, it will use tag only.")]
        [SerializeField] private LayerMask _sheepMask = 0;

        [Tooltip("If true, requires the hit object (or parent) to have tag Sheep.")]
        [SerializeField] private bool _requireSheepTag = true;

        private void Start()
        {
            Destroy(gameObject, _lifeTime);
        }

        private void Update()
        {
            transform.position += transform.forward * (_speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null) return;

            // Optional layer filter
            if (_sheepMask.value != 0)
            {
                int bit = 1 << other.gameObject.layer;
                if ((_sheepMask.value & bit) == 0)
                {
                    // Not a sheep-layer hit -> ignore OR destroy, your choice:
                    // Destroy(gameObject);
                    return;
                }
            }

            // Optional tag filter (walk up the hierarchy)
            Transform t = other.transform;
            if (_requireSheepTag)
            {
                bool foundTag = false;
                while (t != null)
                {
                    if (t.CompareTag("Sheep"))
                    {
                        foundTag = true;
                        break;
                    }
                    t = t.parent;
                }

                if (!foundTag)
                {
                    // Not tagged sheep -> ignore (or destroy if you want)
                    // Destroy(gameObject);
                    return;
                }
            }

            // Find SheepHealth and apply damage
            SheepHealth hp =
                other.GetComponent<SheepHealth>() ??
                other.GetComponentInParent<SheepHealth>() ??
                other.GetComponentInChildren<SheepHealth>();

            if (hp != null && !hp.IsDead)
            {
                hp.ApplyDamage(_damage);
            }

            Destroy(gameObject);
        }
    }
}
