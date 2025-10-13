using System.Collections;
using System.Collections.Generic;

using Core.Shared;

using Gameplay.Player;

using UnityEngine;

public class Rifle : MonoBehaviour, IPlayerTool
{
    [Header("Bolt-Action Settings")]
    [SerializeField, Tooltip("Max amount of ammo in magazine.")] private int maxAmmo = 5;
    [SerializeField, Tooltip("Delay between shots (simulates bolt time)")] private float fireCooldown = 1f;
    [SerializeField, Tooltip("How long the bolt cycle takes")] private float boltCycleTime = 1.5f;
    [SerializeField, Tooltip("Prefab of bullet object.")] private GameObject bulletPrefab;
    [SerializeField, Tooltip("Prefab of bullet object.")] private Transform shotPoint;
    [SerializeField, Tooltip("Damage of rifle.")] private float damage = 0f;


    private int currentAmmo;
    private bool isBoltClosed = true;
    private bool canFire = true;
    private bool isCycling = false;

    private Queue<Bullet> bulletPool = new Queue<Bullet>();
    private PlayerAnimator animator;


    /// <summary>
    /// Initialization method.
    /// </summary>
    public void Initialize(PlayerAnimator animator)
    {
        this.animator = animator;
        currentAmmo = maxAmmo;

        // Initialize pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject b = Instantiate(bulletPrefab);
            b.SetActive(false);
            bulletPool.Enqueue(b.GetComponent<Bullet>());
        }

        gameObject.SetActive(false);
    }

    public void MainUsageStarted(Observable<Vector3> cursorWorldPosition)
    {
        if (!canFire || isCycling)
            return;

        if (!isBoltClosed)
        {
            Debug.Log("Bolt open — cannot fire");
            return;
        }

        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo — reload!");
            return;
        }

        Fire();
    }

    public void MainUsageFinished() { }

    public void SecondaryUsageStarted(Observable<Vector3> cursorWorldPosition) { }

    public void SecondaryUsageFinished() { }

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
        {
            Debug.Log("No bullets available in pool!");
            return;
        }

        Debug.Log("Rifle: Bullet fired!");
        currentAmmo--;
        canFire = false;

        // Get bullet from pool
        Bullet _bullet = bulletPool.Dequeue();
        _bullet.transform.position = shotPoint.position;
        _bullet.transform.forward = shotPoint.forward;
        _bullet.gameObject.SetActive(true);
        _bullet.Shoot(shotPoint.forward);

        // Start the automatic bolt cycle
        StartCoroutine(AutoBoltCycle());
    }

    private IEnumerator ReturnBulletToPool(Bullet bullet, float time)
    {
        yield return new WaitForSeconds(time);
        bullet.gameObject.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    private IEnumerator AutoBoltCycle()
    {
        isCycling = true;
        Debug.Log("Cycling bolt automatically...");

        isBoltClosed = false;
        yield return new WaitForSeconds(boltCycleTime / 3f);

        Debug.Log("Shell ejected");
        yield return new WaitForSeconds(boltCycleTime / 3f);

        if (currentAmmo > 0)
        {
            Debug.Log($"Round chambered ({currentAmmo}/{maxAmmo} left)");
        }
        else
        {
            Debug.Log("No ammo left — reload required");
            isCycling = false;
            isBoltClosed = true;
            canFire = true;
            Debug.Log("Bolt closed, reload available");
            yield break;
        }

        yield return new WaitForSeconds(boltCycleTime / 3f);
        isBoltClosed = true;
        Debug.Log("Bolt closed, ready to fire again");

        // Wait for small cooldown to simulate player resetting aim
        yield return new WaitForSeconds(fireCooldown);

        isCycling = false;
        canFire = true;
    }

    private IEnumerator ReloadRoutine()
    {
        if (currentAmmo == maxAmmo)
        {
            Debug.Log("Magazine full — reload skipped");
            yield break;
        }

        canFire = false;
        Debug.Log("Reloading magazine...");
        yield return new WaitForSeconds(boltCycleTime * 2f);
        currentAmmo = maxAmmo;
        isBoltClosed = true;
        Debug.Log("Reload complete");
        canFire = true;
    }

    public void HideTool()
    {
        gameObject.SetActive(false);
        animator.RemoveHands();
    }

    public void ShowTool()
    {
        gameObject.SetActive(true);
        animator.GetRifle();
    }
}