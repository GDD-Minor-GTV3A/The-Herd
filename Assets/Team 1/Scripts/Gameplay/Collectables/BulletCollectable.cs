using System.Collections;
using Core.Events;
using UnityEngine;

namespace Gameplay.Collectables
{
    [RequireComponent(typeof(Rigidbody))]
    public class BulletCollectable : Collectable
    {
        [SerializeField]
        private int amount = 0;


        private BulletCollectablePool associatedPool;


        public void Initialize(BulletCollectablePool pool = null)
        {
            associatedPool = pool;
            CheckCollider();
            CheckRigidbody();
        }


        public void SetAmount(int amount)
        {
            this.amount = amount; 
        }


        public override void Collect()
        {
            isCollected = true;
            EventManager.Broadcast(new AddBulletsToRifleEvent(amount));

            if (associatedPool != null)
                associatedPool.Release(this);
            else
                Destroy(gameObject);
        }

        public override void Spawn(Vector3 worldPosition, Vector3 forward)
        {
            isCollected = false;
            StartCoroutine(ActivnessRoutine());

            transform.rotation = Quaternion.identity;

            float angleX = Random.Range(45f, 70f);
            float angleY = Random.Range(-180f, 180f);

            Quaternion rotation = Quaternion.Euler(angleX, angleY, 0);

            Vector3 direction = rotation * forward;
            transform.position = worldPosition + direction;

            rb.AddForce(direction * 500, ForceMode.Force);
        }


        private IEnumerator ActivnessRoutine()
        {
            isCollectable = false;

            yield return new WaitForSeconds(1f);

            isCollectable = true;
        }
    }
}