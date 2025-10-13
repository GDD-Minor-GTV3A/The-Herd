using System.Collections;
using System.Collections.Generic;
using Core.Shared;
using UnityEngine;

public class Rifle : MonoBehaviour, IPlayerTool
{
    [Header("Bolt-Action Settings")]
    [SerializeField, Tooltip("Max amount of ammo in magazine.")] private int maxAmmo = 5;
    [SerializeField, Tooltip("Delay between shots (simulates bolt time)")] private float fireCooldown = 1f;
    [SerializeField, Tooltip("How long the bolt cycle takes")] private float boltCycleTime = 1.5f;
    [SerializeField, Tooltip("Prefab of bullet object.")] private GameObject bulletPrefab;
    [SerializeField, Tooltip("Damage of rifle.")] private float damage = 0f;


    private int currentAmmo;
    private bool isBoltClosed = true;
    private bool canFire = true;
    private bool isCycling = false;

    private Queue<Bullet> bulletPool = new Queue<Bullet>();


    /// <summary>
    /// Initialization method.
    /// </summary>
    public void Initialize()
    {
        currentAmmo = maxAmmo;

        // Initialize pool
        for (int i = 0; i < maxAmmo; i++)
        {
            Bullet _bullet = Instantiate(bulletPrefab).GetComponent<Bullet>();
            _bullet.Initialize(damage, bulletPool);
            _bullet.gameObject.SetActive(false);
            bulletPool.Enqueue(_bullet);
        }
    }


    public void MainUsageStarted(Observable<Vector3> cursorWorldPosition)
    {
        if (!canFire || isCycling || !isBoltClosed || currentAmmo <= 0)
            return;

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
            return;

        currentAmmo--;
        canFire = false;

        // Get bullet from pool
        Bullet _bullet = bulletPool.Dequeue();
        _bullet.transform.position = transform.position + transform.forward * 1.5f + Vector3.up * 1.2f;
        _bullet.transform.rotation = Quaternion.identity;
        _bullet.gameObject.SetActive(true);
        _bullet.Shoot(transform.forward);

        // Start the automatic bolt cycle
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

        canFire = false;
        yield return new WaitForSeconds(boltCycleTime * 2f);
        currentAmmo = maxAmmo;
        isBoltClosed = true;
        canFire = true;
    }
}