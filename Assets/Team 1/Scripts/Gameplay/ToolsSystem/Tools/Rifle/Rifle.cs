using System.Collections;

using Core.Events;
using Core.Shared;

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
        /// <summary>
        /// Invokes when current ammo amount changed;
        /// </summary>
        public UnityEvent<int> OnAmmoChanged;

        [Space, Header("Configuration")]
        [SerializeField, Tooltip("Rifle config asset.")]
        private RifleConfig config;

        [SerializeField, Tooltip("Transform from which bullets are spawned.")]
        private Transform shotPoint;

        [SerializeField, Tooltip("Animator component of the rifle.")] 
        private Animator animator;


        // --- Runtime State ---
        private int currentAmmo;
        private bool isBoltClosed = true;
        private bool canFire = true;
        private bool isCycling = false;
        private bool isReloading = false;

        private PlayerAnimator playerAnimator;
        private BulletPool bulletPool;


        public int CurrentAmmo => currentAmmo;
       




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


            // Initialize the bullet pool using values from config
            bulletPool = new BulletPool(config.BulletPrefab, config.Damage, initialCapacity: 0, maxSize: config.MaxPoolSize);

            playerAnimator = animator;
            currentAmmo = config.MaxAmmo;
            OnAmmoChanged?.Invoke(currentAmmo);

             CooldownUI _cooldown = toolUI.GetComponentInChildren<CooldownUI>(true);
            if (_cooldown != null)
            {
                _cooldown.Initialize(config.ReloadTime);
            }
            HideUI();
            HideTool();
        }


        public override void MainUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
            if (!canFire || isCycling || !isBoltClosed || currentAmmo <= 0) return;
            Fire();
        }
        public override void MainUsageFinished() { }

        public override void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition)
        {
            OnSecondaryUse?.Invoke();
            EventManager.Broadcast(new ZoomCameraEvent(20));
            EventManager.Broadcast(new ChangeConePlayerRevealerFOVEvent(-50));
            EventManager.Broadcast(new ChangeConePlayerRevealerDistnaceEvent(50));
        }
        public override void SecondaryUsageFinished()
        {
            EventManager.Broadcast(new ZoomCameraEvent(-20));
            EventManager.Broadcast(new ChangeConePlayerRevealerFOVEvent(50));
            EventManager.Broadcast(new ChangeConePlayerRevealerDistnaceEvent(-50));
        }


        private void Fire()
        {
            if (currentAmmo <= 0 || isReloading || !canFire) return;

            currentAmmo--;
            OnAmmoChanged?.Invoke(currentAmmo);
            canFire = false;

            Bullet _bullet = bulletPool.Get();
            _bullet.transform.SetPositionAndRotation(shotPoint.position, shotPoint.rotation);
            _bullet.Shoot(shotPoint.forward);

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

            if (currentAmmo <= 0)
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
            if (!isCycling && !isReloading)
                StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            if (currentAmmo == config.MaxAmmo) yield break;

            canFire = false;
            isReloading = true;


            OnReload?.Invoke();
            animator.SetTrigger("Reload");


            yield return new WaitForSeconds(config.ReloadTime);

            currentAmmo = config.MaxAmmo;
            OnAmmoChanged?.Invoke(currentAmmo);
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
    }
}