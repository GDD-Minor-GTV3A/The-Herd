// using System.Collections;
// using Gameplay.Collectables;
// using Gameplay.Inventory;
// using UnityEngine;

// namespace Team8.Collectables
// {
//     [RequireComponent(typeof(Rigidbody))]
//     public class ScrollCollectable : Collectable
//     {
//         [Header("Scroll Configuration")]
//         [SerializeField] private InventoryItem scrollItem;
//         [SerializeField] private int amount = 1;

//         private void Start()
//         {
//             Initialize();
//         }

//         private void Initialize()
//         {
//             CheckCollider();
//             CheckRigidbody();
//         }

//         public override void Collect()
//         {
//             if (scrollItem == null)
//             {
//                 Debug.LogWarning("ScrollCollectable: No scroll item assigned!");
//                 return;
//             }

//             if (scrollItem.category != ItemCategory.Scroll)
//             {
//                 Debug.LogWarning($"ScrollCollectable: Item {scrollItem.itemName} is not a Scroll category!");
//                 return;
//             }


//             isCollected = true;

//             // add scroll to player inventory
//             PlayerInventory.Instance.AddItem(scrollItem, amount);

//             // destroy the collectable object
//             Destroy(gameObject);
//         }

//         public override void Spawn(Vector3 worldPosition, Vector3 forward)
//         {
//             isCollected = false;
//             StartCoroutine(ActivnessRoutine());

//             transform.rotation = Quaternion.identity;

//             // randomized orientation
//             float angleX = Random.Range(45f, 70f);
//             float angleY = Random.Range(-180f, 180f);

//             Quaternion rotation = Quaternion.Euler(angleX, angleY, 0);
//             Vector3 direction = rotation * forward;

//             transform.position = worldPosition + direction;
//             rb.AddForce(direction * 300, ForceMode.Force);
//         }

//         private IEnumerator ActivnessRoutine()
//         {
//             isCollectable = false;
//             yield return new WaitForSeconds(1f);
//             isCollectable = true;
//         }

//         public void SetScrollItem(InventoryItem item)
//         {
//             scrollItem = item;
//         }

//         public void SetAmount(int newAmount)
//         {
//             amount = newAmount;
//         }
//     }
// }
