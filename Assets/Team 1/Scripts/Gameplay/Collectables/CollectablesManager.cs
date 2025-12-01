using System.Collections.Generic;
using System.Linq;
using Core.Events;
using UnityEngine;

namespace Gameplay.Collectables 
{
    public class CollectablesManager : MonoBehaviour
    {
        [Header("Bullets")]
        [SerializeField]
        private BulletCollectable bulletCollectablePrefab;


        private List<BulletCollectable> bulletCollectablesOnScene;
        private BulletCollectablePool bulletsPool;


        public void Initialize()
        {
            bulletsPool = new BulletCollectablePool(bulletCollectablePrefab);

            EventManager.AddListener<SpawnNewBulletCollectableEvent>(SpawnNewBulletCollectable);
            bulletCollectablesOnScene = FindObjectsByType<BulletCollectable>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();

            foreach (BulletCollectable bulletCollectable in bulletCollectablesOnScene)
                bulletCollectable.Initialize();
        }


        private void SpawnNewBulletCollectable(SpawnNewBulletCollectableEvent evt)
        {
            BulletCollectable newBulletCollectable = bulletsPool.Get();
            newBulletCollectable.SetAmount(evt.Amount);
            newBulletCollectable.Spawn(evt.WorldPosition, evt.Forward);
        }
    }
}