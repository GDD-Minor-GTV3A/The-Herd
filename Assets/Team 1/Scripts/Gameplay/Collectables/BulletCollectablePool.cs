using Core.Shared;
using UnityEngine;

namespace Gameplay.Collectables
{
    public class BulletCollectablePool : GenericPool<BulletCollectable>
    {
        BulletCollectable prefab;


        public BulletCollectablePool(BulletCollectable prefab, int initialCapacity = 0, int maxSize = 50)
            : base(initialCapacity, maxSize)
        {
            this.prefab = prefab;
        }


        protected override BulletCollectable Create()
        {
            BulletCollectable newBulletCollectable = GameObject.Instantiate(prefab);
            newBulletCollectable.Initialize(this);
            return newBulletCollectable;
        }

        protected override void OnDestroy(BulletCollectable item)
        {
            GameObject.Destroy(item.gameObject);
        }

        protected override void OnGet(BulletCollectable item)
        {
            item.gameObject.SetActive(true);
        }

        protected override void OnRelease(BulletCollectable item)
        {
            item.gameObject.SetActive(false);
        }
    }
}
