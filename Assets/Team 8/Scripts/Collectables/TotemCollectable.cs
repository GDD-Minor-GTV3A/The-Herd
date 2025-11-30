using System.Collections;
using Gameplay.Collectables;
using Gameplay.Inventory;
using UnityEngine;

namespace Team8.Collectables
{
    [RequireComponent(typeof(Rigidbody))]
    public class TotemCollectable : Collectable
    {
        [Header("Totem Configuration")]
        [SerializeField] private InventoryItem totemItem;
        [SerializeField] private int amount = 1;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            CheckCollider();
            CheckRigidbody();
        }

        public override void Collect()
        {
            if (totemItem == null)
            {
                Debug.LogWarning("TotemCollectable: No totem item assigned!");
                return;
            }

            if (totemItem.category != ItemCategory.ReviveTotem)
            {
                Debug.LogWarning($"TotemCollectable: Item {totemItem.itemName} is not a ReviveTotem category!");
                return;
            }

            isCollected = true;

            // add totem to player inventory
            PlayerInventory.Instance.AddItem(totemItem, amount);

            // destroy the collectable object
            Destroy(gameObject);
        }

        public override void Spawn(Vector3 worldPosition, Vector3 forward)
        {
            isCollected = false;
            StartCoroutine(ActivnessRoutine());

            transform.rotation = Quaternion.identity;

            // randomized orientation
            float angleX = Random.Range(45f, 70f);
            float angleY = Random.Range(-180f, 180f);

            Quaternion rotation = Quaternion.Euler(angleX, angleY, 0);
            Vector3 direction = rotation * forward;

            transform.position = worldPosition + direction;
            rb.AddForce(direction * 300, ForceMode.Force);
        }

        private IEnumerator ActivnessRoutine()
        {
            isCollectable = false;
            yield return new WaitForSeconds(1f);
            isCollectable = true;
        }

        public void SetTotemItem(InventoryItem item)
        {
            totemItem = item;
        }

        public void SetAmount(int newAmount)
        {
            amount = newAmount;
        }
    }
}
