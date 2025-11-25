using System.Collections;
using Core.Events;
using Core.Shared;
using CustomEditor.Attributes;
using Gameplay.Player;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.ToolsSystem.Tools.Rifle
{
    /// <summary>
    /// Handles logic of rifle tool.
    /// </summary>
    public class Rifle : PlayerTool
    {
        [Header("Configuration")]
        [SerializeField, Tooltip("Rifle config asset.")]
        private RifleConfig config;

        [SerializeField, Tooltip("Transform from which bullets are spawned.")]
        private Transform shotPoint;

        [SerializeField, Tooltip("Animator component of the rifle.")] 
        private Animator animator;

        


        // --- Runtime State ---
        private uint freeAmmo;
        private int currentMagazineAmmo;
        private int magazineCapacity;
        private bool isBoltClosed = true;
        private bool canFire = true;
        private bool isCycling = false;
        private bool isReloading = false;
        private bool isAiming = false;

        private PlayerAnimator playerAnimator;
        private BulletPool bulletPool;


        /// <summary>
        /// Invokes when current ammo amount changed;
        /// </summary>
        public UnityEvent<int, uint> OnAmmoChanged;


        public int CurrentAmmo => currentMagazineAmmo;


        [Space, Header("Debug")]
        [SerializeField, Tooltip("Show fire spread conus. Shot point and config has to be assigned.")]
        private bool showFireSpread;
        [SerializeField, ShowIf("showFireSpread"), Range(10f, 500f)]
        private float fireSpreadLinesLength = 10;


        /// <summary>
        /// Initialization method.
        /// </summary>
        public void Initialize(PlayerAnimator animator)
        {
            if (config == null)
            {
                Debug.LogError("RifleConfig not assigned in inspector!");
                enabled = false;
                return;
            }

            isAiming = false;


            // Initialize the bullet pool using values from config
            bulletPool = new BulletPool(config.BulletPrefab, config.Damage, initialCapacity: 0, maxSize: config.MaxPoolSize);

            playerAnimator = animator;
            magazineCapacity = config.MaxAmmo;
            currentMagazineAmmo = magazineCapacity;
            freeAmmo = config.InitialFreeAmmo;

            OnAmmoChanged?.Invoke(currentMagazineAmmo, freeAmmo);

             CooldownUI _cooldown = toolUI.GetComponentInChildren<CooldownUI>(true);
            if (_cooldown != null)
            {
                _cooldown.Initialize(config.ReloadTime);
            }
            HideUI();
            HideTool();
            EventManager.AddListener<AddBulletsToRifleEvent>(AddFreeBullets);
        }


        private void AddFreeBullets(AddBulletsToRifleEvent evt)
        {
            freeAmmo += (uint)evt.Amount;
            OnAmmoChanged?.Invoke(currentMagazineAmmo, freeAmmo);
        }


        public override void MainUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
            if (!canFire || isCycling || !isBoltClosed || currentMagazineAmmo <= 0) return;
            Fire();
        }
        public override void MainUsageFinished() { }

        public override void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
            OnSecondaryUse?.Invoke();
            EventManager.Broadcast(new ZoomCameraEvent(20));
            EventManager.Broadcast(new ChangeConePlayerRevealerFOVEvent(-50));
            EventManager.Broadcast(new ChangeConePlayerRevealerDistnaceEvent(50));
            isAiming = true;
        }
        public override void SecondaryUsageFinished()
        {
            EventManager.Broadcast(new ZoomCameraEvent(-20));
            EventManager.Broadcast(new ChangeConePlayerRevealerFOVEvent(50));
            EventManager.Broadcast(new ChangeConePlayerRevealerDistnaceEvent(-50));
            isAiming = false;
        }


        private void Fire()
        {
            if (!config.IsInfiniteMagazine && (currentMagazineAmmo <= 0 || isReloading || !canFire)) return;

            if (!config.IsInfiniteMagazine)
            {
                currentMagazineAmmo--;
                OnAmmoChanged?.Invoke(currentMagazineAmmo, freeAmmo);
            }

            canFire = false;

            Bullet _bullet = bulletPool.Get();
            _bullet.transform.SetPositionAndRotation(shotPoint.position, shotPoint.rotation);

            Vector3 bulletDirection = shotPoint.forward;

            if (!isAiming)
            {
                float halfAngle = config.SpreadAngle / 2;
                bulletDirection = Quaternion.AngleAxis(Random.Range(-halfAngle, halfAngle), shotPoint.up) * bulletDirection;
            }

            _bullet.Shoot(bulletDirection);

            OnMainUse?.Invoke();
            animator.SetTrigger("BoltCycle");
            StartCoroutine(AutoBoltCycleRoutine());
        }


        private IEnumerator AutoBoltCycleRoutine()
        {
            isCycling = true;
            isBoltClosed = false;

            float _step = config.BoltCycleTime / 3f;

            yield return new WaitForSeconds(_step); // open bolt
            yield return new WaitForSeconds(_step); // eject round

            if (currentMagazineAmmo <= 0)
            {
                isCycling = false;
                isBoltClosed = true;
                canFire = true;
                yield break;
            }

            yield return new WaitForSeconds(_step); // close bolt

            isBoltClosed = true;
            isCycling = false;
            canFire = true;
        }


        public override void Reload()
        {
            if (!isCycling && !isReloading && !config.IsInfiniteMagazine)
                StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            if (currentMagazineAmmo == magazineCapacity) yield break;
            if (freeAmmo == 0 && !config.IsInfiniteAmmo) yield break;


            uint ammoToLoad;

            if (!config.IsInfiniteAmmo)
                ammoToLoad = (uint)Mathf.Clamp((int)freeAmmo, 0, magazineCapacity);
            else
                ammoToLoad = (uint)magazineCapacity;

            EventManager.Broadcast(new SpawnNewBulletCollectableEvent(transform.position, shotPoint.forward, currentMagazineAmmo));
            
            currentMagazineAmmo = 0;



            OnAmmoChanged?.Invoke(currentMagazineAmmo, freeAmmo);

            canFire = false;
            isReloading = true;


            OnReload?.Invoke();
            animator.SetTrigger("Reload");


            yield return new WaitForSeconds(config.ReloadTime);

            if (!config.IsInfiniteAmmo)
                freeAmmo -= ammoToLoad;

            currentMagazineAmmo = (int)ammoToLoad;
            OnAmmoChanged?.Invoke(currentMagazineAmmo, freeAmmo);
            isBoltClosed = true;
            canFire = true;
            isReloading = false;
        }


        public override void HideTool()
        {
            base.HideTool();
            isBoltClosed = true;
            canFire = true;
            isCycling = false;
            isReloading = false;

            gameObject.SetActive(false);
            playerAnimator.RemoveHands();
        }

        public override void ShowTool()
        {
            base.ShowTool();
            gameObject.SetActive(true);
            playerAnimator.GetTool(keyPoints);
            animator.SetFloat("BoltCycleSpeed", 1 / config.BoltCycleTime);
            animator.SetFloat("ReloadSpeed", 1 / config.ReloadTime);
        }


        public void AddBullets(uint bulletsAmount)
        {
            freeAmmo += bulletsAmount;
        }


        private void OnDrawGizmos()
        {
            if (showFireSpread && config != null && shotPoint != null)
            {

                float halfAngle = config.SpreadAngle / 2;

                Vector3 leftEdge = Quaternion.AngleAxis(-halfAngle, shotPoint.up) * shotPoint.forward;
                Vector3 rightEdge = Quaternion.AngleAxis(halfAngle, shotPoint.up) * shotPoint.forward;

                leftEdge *= fireSpreadLinesLength;
                rightEdge *= fireSpreadLinesLength;

                Gizmos.color = Color.red;
                Gizmos.DrawRay(shotPoint.position, leftEdge);
                Gizmos.DrawRay(shotPoint.position, rightEdge);
            }

        }
    }
}