using System.Collections;

using Core.Shared;

using Gameplay.Player;

using UnityEngine;

public class Rifle : MonoBehaviour, IPlayerTool
{
    [Header("Bolt-Action Settings")]
    [SerializeField] private int maxAmmo = 5;
    [SerializeField] private float reload = 5f;
    [SerializeField] private float boltCycleTime = 1.5f;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private float damage = 0f;

    [Header("Animation Points")]
    [SerializeField] private ToolAnimationKeyPoints keyPoints;

    private int currentAmmo;
    private bool isBoltClosed = true;
    private bool canFire = true;
    private bool isCycling = false;

    private PlayerAnimator animator;
    private GenericPool<Bullet> bulletPool;

    private void Awake()
    {
        bulletPool = new GenericPool<Bullet>(
            createFunc: () =>
            {
                Bullet b = Instantiate(bulletPrefab);
                b.Initialize(damage, ReleaseBullet); // callback for returning
                b.gameObject.SetActive(false);
                return b;
            },
            onGet: b => b.gameObject.SetActive(true),
            onRelease: b => b.gameObject.SetActive(false),
            onDestroy: b => Destroy(b.gameObject),
            initialCapacity: 0,  // <-- start empty, no bullets pre-created
            maxSize: 50
        );
    }
    private void ReleaseBullet(Bullet bullet)
    {
        bulletPool.Release(bullet);
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
