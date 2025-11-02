using System.Collections;
using System.Diagnostics.CodeAnalysis;

using Core.Shared;
using Core.Shared.Utilities;

using Gameplay.Player;

using UnityEngine;

public class Rifle : MonoBehaviour, IPlayerTool
{
    [Header("Bolt-Action Settings")]

    [Tooltip("Maximum number of rounds the rifle can hold at once.")]
    [SerializeField] private int maxAmmo = 5;

    [Tooltip("Total time (in seconds) to fully reload the rifle.")]
    [SerializeField] private float reload = 5f;

    [Tooltip("Total duration (in seconds) for a complete bolt cycle (open, eject, close).")]
    [SerializeField] private float boltCycleTime = 1.5f;

    [Tooltip("Prefab reference for the bullet this rifle fires. Required.")]
    [SerializeField, NotNull] private Bullet bulletPrefab;

    [Tooltip("Transform from which bullets are spawned and oriented when firing. Required.")]
    [SerializeField, NotNull] private Transform shotPoint;

    [Tooltip("Damage dealt per bullet fired.")]
    [SerializeField] private float damage = 0f;


    [Header("Animation Points")]

    [Tooltip("Defines the key points in the player's animation for this specific tool.")]
    [SerializeField, NotNull] private ToolAnimationKeyPoints keyPoints;
    
    private int currentAmmo;
    private bool isBoltClosed = true;
    private bool canFire = true;
    private bool isCycling = false;
    private bool isReloading = false;

    private PlayerAnimator animator;
    private BulletPool bulletPool;

    private void Awake()
    {
        // Initialize the BulletPool, starts empty (lazy-loaded)
        bulletPool = new BulletPool(bulletPrefab, damage, initialCapacity: 0, maxSize: 50);
    }

    public void Initialize(PlayerAnimator animator)
    {
        this.animator = animator;
        currentAmmo = maxAmmo;
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
        if (currentAmmo <= 0) return;

        if (isReloading) return;

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

        yield return new WaitForSeconds(boltCycleTime / 3f);
        yield return new WaitForSeconds(boltCycleTime / 3f);

        if (currentAmmo <= 0)
        {
            isCycling = false;
            isBoltClosed = true;
            canFire = true;
            yield break;
        }

        yield return new WaitForSeconds(boltCycleTime / 3f);

        isBoltClosed = true;
        isCycling = false;
        canFire = true;
    }

    public void Reload()
    {
        if (!isCycling) StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        if (currentAmmo == maxAmmo) yield break;

        if (isReloading) yield break;

        canFire = false;
        isReloading = true;
        yield return new WaitForSeconds(reload);

        currentAmmo = maxAmmo;
        isBoltClosed = true;
        canFire = true;
        isReloading = false;
    }

    public void HideTool()
    {
        gameObject.SetActive(false);
        animator.RemoveHands();
    }

    public void ShowTool()
    {
        gameObject.SetActive(true);
        animator.GetTool(keyPoints);
    }
}
