using UnityEngine;

namespace Game.Scripts.Gameplay.Projectiles.Amalgamation
{
  
    public class BulletScript : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private float _speed = 15f;

        [SerializeField]
        private float _lifeTime = 3f;

        #endregion


        #region Unity Methods

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
            // Reminder for self: Add damage logic, check tags, collision.
            Destroy(gameObject);
        }

        #endregion
    }
}
