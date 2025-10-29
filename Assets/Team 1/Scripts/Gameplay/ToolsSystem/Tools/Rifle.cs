using System.Collections;

using Core.Shared;

using Gameplay.Player;

using UnityEngine;
using UnityEngine.Pool;

public class Rifle : MonoBehaviour, IPlayerTool
{
    [Header("Bolt-Action Settings")]
    [SerializeField, Tooltip("Max amount of ammo in magazine.")] private int maxAmmo = 5;
    [SerializeField, Tooltip("Amount of time it takes to reload the weapon")] private float reload = 5f;
    [SerializeField, Tooltip("How long the bolt cycle takes")] private float boltCycleTime = 1.5f;
    [SerializeField, Tooltip("Prefab of bullet object.")] private Bullet bulletPrefab;
    [SerializeField, Tooltip("Spawn point for bullets.")] private Transform shotPoint;
    [SerializeField, Tooltip("Damage of rifle.")] private float damage = 0f;

    [Space]
    [Header("Animation Points")]
    [SerializeField] private ToolAnimationKeyPoints keyPoints;

    private int currentAmmo;
    private bool isBoltClosed = true;
    private bool canFire = true;
    private bool isCycling = false;

    private PlayerAnimator animator;
    private IObjectPool<Bullet> pool;

    private void Awake()
    {
        // Initialize the pool with callbacks.
        pool = new ObjectPool<Bullet>(
            createFunc: CreateBullet,
            actionOnGet: OnGetBullet,
            actionOnRelease: OnReleaseBullet,
            actionOnDestroy: OnDestroyBullet,
            collectionCheck: true,
            defaultCapacity: maxAmmo,
            maxSize: 50
        );
    }

    public void Initialize(PlayerAnimator animator)
    {
        this.animator = animator;
        currentAmmo = maxAmmo;
        gameObject.SetActive(false);
    }

    private Bullet CreateBullet()
    {

        Bullet bullet = Instantiate(bulletPrefab);
        bullet.Initialize(damage, pool);
        bullet.gameObject.SetActive(false);
        return bullet;
    }


    private void OnGetBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void OnReleaseBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
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

    private void Fire()
    {
        if (currentAmmo <= 0)
            return;

        currentAmmo--;
        canFire = false;

        Bullet bullet = pool.Get();
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
        if (!isCycling)
            StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        if (currentAmmo == maxAmmo)
            yield break;

        canFire = false;
        yield return new WaitForSeconds(reload);
        currentAmmo = maxAmmo;
        isBoltClosed = true;
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
        animator.GetTool(keyPoints);
    }
}
