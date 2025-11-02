using System.Collections;
using System.Collections.Generic;

using Core.Events;
using Core.Shared;
using Core.Shared.Utilities;

using DG.Tweening;

using Gameplay.CameraSettings;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;

public class Rifle : MonoBehaviour, IPlayerTool
{
    [Header("Bolt-Action Settings")]
    [SerializeField, Tooltip("Max amount of ammo in magazine.")] private int maxAmmo = 5;
    [SerializeField, Tooltip("Delay between shots (simulates bolt time)")] private float fireCooldown = 1f;
    [SerializeField, Tooltip("How long the bolt cycle takes")] private float boltCycleTime = 1.5f;
    [SerializeField, Tooltip("Prefab of bullet object."), Required] private GameObject bulletPrefab;
    [SerializeField, Tooltip("Prefab of bullet object."), Required] private Transform shotPoint;
    [SerializeField, Tooltip("Damage of rifle.")] private float damage = 0f;

    [Space]
    [SerializeField] private UnityEvent onShot;

    [Space]
    [Header("Animation Points")]
    [SerializeField] private ToolAnimationKeyPoints keyPoints;


    private int currentAmmo;
    private bool isBoltClosed = true;
    private bool canFire = true;
    private bool isCycling = false;

    private Queue<Bullet> bulletPool = new Queue<Bullet>();
    private PlayerAnimator playerAnimator;
    [SerializeField] private Animator animator;


    /// <summary>
    /// Initialization method.
    /// </summary>
    public void Initialize(PlayerAnimator playerAnimator)
    {
        this.playerAnimator = playerAnimator;
        currentAmmo = maxAmmo;

        //animator = GetComponent<Animator>();


        // Initialize pool
        for (int i = 0; i < maxAmmo; i++)
        {
            Bullet _bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
            _bullet.Initialize(damage, bulletPool);
            _bullet.gameObject.SetActive(false);
            bulletPool.Enqueue(_bullet);
        }

        gameObject.SetActive(false);
    }


    public void MainUsageStarted(Observable<Vector3> cursorWorldPosition)
    {
        if (!canFire || isCycling || !isBoltClosed || currentAmmo <= 0)
            return;

        Fire();
    }

    public void MainUsageFinished() { }

    public void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition) 
    {
        EventManager.Broadcast(new ZoomCameraEvent(20));
        EventManager.Broadcast(new ChangeConePlayerRevealerFOVEvent(-50));
        EventManager.Broadcast(new ChangeConePlayerRevealerDistnaceEvent(50));
    }

    public void SecondaryUsageFinished() 
    {
        EventManager.Broadcast(new ZoomCameraEvent(-20));
        EventManager.Broadcast(new ChangeConePlayerRevealerFOVEvent(50));
        EventManager.Broadcast(new ChangeConePlayerRevealerDistnaceEvent(-50));


    }

    public void Reload()
    {
        if (!isCycling)
        {
            StartCoroutine(ReloadRoutine());
        }
    }


    private void Fire()
    {
        if (bulletPool.Count == 0)
            return;

        currentAmmo--;
        canFire = false;

        // Get bullet from pool
        Bullet _bullet = bulletPool.Dequeue();
        _bullet.transform.position = shotPoint.position;
        _bullet.transform.forward = shotPoint.forward;
        _bullet.gameObject.SetActive(true);
        _bullet.Shoot(shotPoint.forward);

        onShot?.Invoke();

        // Start the automatic bolt cycle
        animator.SetTrigger("BoltCycle");
        StartCoroutine(AutoBoltCycle());
    }


    private IEnumerator AutoBoltCycle()
    {
        isCycling = true;

        isBoltClosed = false;
        yield return new WaitForSeconds(boltCycleTime / 3f);

        yield return new WaitForSeconds(boltCycleTime / 3f);

        if (currentAmmo > 0)
        {
        }
        else
        {
            isCycling = false;
            isBoltClosed = true;
            canFire = true;
            yield break;
        }

        yield return new WaitForSeconds(boltCycleTime / 3f);
        isBoltClosed = true;

        // Wait for small cooldown to simulate player resetting aim
        yield return new WaitForSeconds(fireCooldown);

        isCycling = false;
        canFire = true;
    }


    private IEnumerator ReloadRoutine()
    {
        if (currentAmmo == maxAmmo)
            yield break;

        animator.SetTrigger("Reload");
        canFire = false;
        yield return new WaitForSeconds(boltCycleTime * 2f);
        currentAmmo = maxAmmo;
        isBoltClosed = true;
        canFire = true;
    }

    public void HideTool()
    {
        gameObject.SetActive(false);
        playerAnimator.RemoveHands();
    }

    public void ShowTool()
    {
        gameObject.SetActive(true);
        playerAnimator.GetTool(keyPoints);
        animator.SetFloat("BoltCycleSpeed", 1 / (boltCycleTime + fireCooldown));
        animator.SetFloat("ReloadSpeed", 1 / (boltCycleTime * 2f));
    }
}