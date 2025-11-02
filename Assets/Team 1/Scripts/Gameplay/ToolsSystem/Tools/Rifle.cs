using System;
using System.Collections;

using Core.Shared;
using Core.Shared.Utilities;

using Gameplay.Player;

using UnityEngine;

public class Rifle : MonoBehaviour, IPlayerTool
{
    [Header("Configuration")]
    [SerializeField, Tooltip("Rifle config asset.")]
    private RifleConfig config;

    [SerializeField, Tooltip("Transform from which bullets are spawned.")]
    private Transform shotPoint;

    [Header("Animation Points")]
    [SerializeField, Tooltip("Defines the key points in the player's animation for this specific tool.")]
    private ToolAnimationKeyPoints keyPoints;

    // --- Runtime State ---
    private int currentAmmo;
    private bool isBoltClosed = true;
    private bool canFire = true;
    private bool isCycling = false;
    private bool isReloading = false;

    private PlayerAnimator animator;
    private BulletPool bulletPool;

    private void Awake()
    {
        if (config == null)
        {
            Debug.LogError("RifleConfig not assigned in inspector!");
            enabled = false;
            return;
        }

        // Initialize the bullet pool using values from config
        bulletPool = new BulletPool(config.BulletPrefab, config.Damage, initialCapacity: 0, maxSize: config.MaxPoolSize);
    }

    public void Initialize(PlayerAnimator animator)
    {
        this.animator = animator;
        currentAmmo = config.MaxAmmo;
        gameObject.SetActive(false);
    }

    public void MainUsageStarted(Observable<Vector3> cursorWorldPosition)
    {
        if (!canFire || isCycling || !isBoltClosed || currentAmmo <= 0) return;
        Fire();
    }

    public void MainUsageFinished() { }
    public void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition) { }
    public void SecondaryUsageFinished() { }

    private void Fire()
    {
        if (currentAmmo <= 0 || isReloading || !canFire) return;

        currentAmmo--;
        canFire = false;

        Bullet bullet = bulletPool.Get();
        bullet.transform.SetPositionAndRotation(shotPoint.position, shotPoint.rotation);
        bullet.Shoot(shotPoint.forward);

        StartCoroutine(AutoBoltCycle());
    }

    private IEnumerator AutoBoltCycle()
    {
        isCycling = true;
        isBoltClosed = false;

        float step = config.BoltCycleTime / 3f;

        yield return new WaitForSeconds(step); // open bolt
        yield return new WaitForSeconds(step); // eject round

        if (currentAmmo <= 0)
        {
            isCycling = false;
            isBoltClosed = true;
            canFire = true;
            yield break;
        }

        yield return new WaitForSeconds(step); // close bolt

        isBoltClosed = true;
        isCycling = false;
        canFire = true;
    }

    public void Reload()
    {
        if (!isCycling && !isReloading)
            StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        if (currentAmmo == config.MaxAmmo) yield break;

        canFire = false;
        isReloading = true;
        yield return new WaitForSeconds(config.ReloadTime);

        currentAmmo = config.MaxAmmo;
        isBoltClosed = true;
        canFire = true;
        isReloading = false;
    }

    public void HideTool()
    {
        isBoltClosed = true;
        canFire = true;
        isCycling = false;
        isReloading = false;

        gameObject.SetActive(false);
        animator.RemoveHands();
    }

    public void ShowTool()
    {
        gameObject.SetActive(true);
        animator.GetTool(keyPoints);
    }
}
