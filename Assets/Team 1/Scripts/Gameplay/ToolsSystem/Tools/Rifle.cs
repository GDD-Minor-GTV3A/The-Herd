using System.Collections;
using Core.Events;
using Core.Shared;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;

public class Rifle : PlayerTool
{
    [Header("Configuration")]
    [SerializeField, Tooltip("Rifle config asset.")]
    private RifleConfig config;

    [SerializeField, Tooltip("Transform from which bullets are spawned.")]
    private Transform shotPoint;

    [Header("Animation Points")]
    [SerializeField, Tooltip("Defines the key points in the player's animation for this specific tool.")]
    private ToolAnimationKeyPoints keyPoints;
    [SerializeField] private Animator animator;


    [Space]
    [SerializeField] private UnityEvent onShot;


    // --- Runtime State ---
    private int currentAmmo;
    private bool isBoltClosed = true;
    private bool canFire = true;
    private bool isCycling = false;
    private bool isReloading = false;

    private PlayerAnimator playerAnimator;
    private BulletPool bulletPool;

    public int CurrentAmmo => currentAmmo;

    public void Initialize(PlayerAnimator animator)
    {
        if (config == null)
        {
            Debug.LogError("RifleConfig not assigned in inspector!");
            enabled = false;
            return;
        }

        HideUI();

        // Initialize the bullet pool using values from config
        bulletPool = new BulletPool(config.BulletPrefab, config.Damage, initialCapacity: 0, maxSize: config.MaxPoolSize);

        playerAnimator = animator;
        currentAmmo = config.MaxAmmo;
        gameObject.SetActive(false);
    }

    public override void MainUsageStarted(Observable<Vector3> cursorWorldPosition)
    {
        if (!canFire || isCycling || !isBoltClosed || currentAmmo <= 0) return;
        Fire();
    }

    public override void MainUsageFinished() { }
    public override void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition) 
    {
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
        canFire = false;

        Bullet bullet = bulletPool.Get();
        bullet.transform.SetPositionAndRotation(shotPoint.position, shotPoint.rotation);
        bullet.Shoot(shotPoint.forward);

        onShot?.Invoke();
        animator.SetTrigger("BoltCycle");
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

    public override void Reload()
    {
        if (!isCycling && !isReloading)
            StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        if (currentAmmo == config.MaxAmmo) yield break;

        animator.SetTrigger("Reload");

        canFire = false;
        isReloading = true;
        yield return new WaitForSeconds(config.ReloadTime);

        currentAmmo = config.MaxAmmo;
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