using Core.Shared;
using UnityEngine;
using System.Collections;

public class Rifle : MonoBehaviour, IPlayerTool
{
    [Header("Bolt-Action Settings")]
    [SerializeField] private int maxAmmo = 5;
    [SerializeField] private float fireCooldown = 1f; // Delay between shots (simulates bolt time)
    [SerializeField] private float boltCycleTime = 1.5f; // How long the bolt cycle takes
    [SerializeField] private GameObject bulletPrefab;

    private int currentAmmo;
    private bool isBoltClosed = true;
    private bool canFire = true;
    private bool isCycling = false;

    private void Start()
    {
        currentAmmo = maxAmmo;
        Debug.Log("Rifle created and ready");
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
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab not set!");
            return;
        }

        Debug.Log("Rifle: Bullet fired!");
        currentAmmo--;
        canFire = false;

        // Spawn bullet at player position (or slightly in front)
        Vector3 spawnPosition = transform.position + transform.forward * 1.5f + Vector3.up * 1.2f; // adjust as needed
        GameObject bulletObj = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // Shoot in player's forward direction
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Shoot(transform.forward);
        }

        // Start bolt cycle
        StartCoroutine(AutoBoltCycle());
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
}