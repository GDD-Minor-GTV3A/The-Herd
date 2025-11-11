using System.Collections;
using Core.Events;
using Core.Shared;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

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
    [Header("UI")]
    [SerializeField] private Slider reloadSlider;
    [SerializeField] private GameObject reloadUIRoot;
    private Coroutine reloadCo;

    [Space]
    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip reloadSFX;


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
            reloadCo = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        if (currentAmmo == config.MaxAmmo) yield break;

        canFire = false;
        isReloading = true;

        SetReloadUIVisible(true);
        SetReloadProgress(1f);

        animator.SetTrigger("Reload");

        if (audioSource != null && reloadSFX != null)
            audioSource.PlayOneShot(reloadSFX);

        float duration = config.ReloadTime;
        float t = 0f;

        while (t < duration)
        {
            if (!gameObject.activeInHierarchy) { CleanupReloadUICancel(); yield break; }

            t += Time.deltaTime;

            
            float remaining = Mathf.Clamp01(1f - (t / duration));
            SetReloadProgress(remaining);

            yield return null;
        }

        //yield return new WaitForSeconds(config.ReloadTime);
        
        currentAmmo = config.MaxAmmo;
        isBoltClosed = true;
        canFire = true;
        isReloading = false;

        SetReloadProgress(0f);
        SetReloadUIVisible(false);
        reloadCo = null;
    }

    public override void HideTool()
    {
        if (reloadCo != null) { StopCoroutine(reloadCo); reloadCo = null; }
        SetReloadUIVisible(false);

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
        
        SetReloadUIVisible(false);
        SetReloadProgress(1f);

        base.ShowTool();
        gameObject.SetActive(true);
        playerAnimator.GetTool(keyPoints);
        animator.SetFloat("BoltCycleSpeed", 1 / config.BoltCycleTime);
        animator.SetFloat("ReloadSpeed", 1 / config.ReloadTime);

    }

    private void SetReloadUIVisible(bool visible)
    {
        if (reloadUIRoot != null) reloadUIRoot.SetActive(visible);
        else if (reloadSlider != null) reloadSlider.gameObject.SetActive(visible);
    }

    private void SetReloadProgress(float v)
    {
        if (reloadSlider != null) reloadSlider.value = v; 
    }

    private void CleanupReloadUICancel()
    {
        isReloading = false;
        canFire = true;
        SetReloadUIVisible(false);
        reloadCo = null;
    }
}