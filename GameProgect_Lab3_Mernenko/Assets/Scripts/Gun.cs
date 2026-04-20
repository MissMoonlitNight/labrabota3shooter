using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Реализует механику стрельбы, перезарядки, отдачи и разброса для конкретного оружия.
/// </summary>
public class Gun : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float range = 100f;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int totalAmmo = 90;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Recoil & Spread")]
    [SerializeField] private float recoilAmount = 2f;
    [SerializeField] private float recoilRecovery = 5f;
    [SerializeField] private float spread = 0.5f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private AudioSource reloadSound;

    [Header("UI")]
    [SerializeField] private Text ammoText;

    private int currentAmmo;
    private float nextFireTime = 0f;
    private bool isReloading = false;
    private Camera playerCamera;
    private float currentRecoil = 0f;

    private void Start()
    {
        playerCamera = Camera.main;
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    private void Update()
    {
        // Плавное возвращение отдачи к нулю
        if (currentRecoil > 0)
        {
            currentRecoil -= recoilRecovery * Time.deltaTime;
            if (currentRecoil < 0) currentRecoil = 0;
        }

        // Не обрабатываем стрельбу, если оружие неактивно или идёт перезарядка
        if (!gameObject.activeSelf || isReloading) return;

        // Стрельба
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        // Перезарядка
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        if (currentAmmo <= 0)
        {
            if (totalAmmo > 0 && !isReloading) StartCoroutine(Reload());
            return;
        }

        currentAmmo--;
        UpdateAmmoUI();

        if (muzzleFlash != null) muzzleFlash.Play();
        if (shootSound != null) shootSound.Play();

        // Отдача
        currentRecoil += recoilAmount;
        currentRecoil = Mathf.Clamp(currentRecoil, 0, 15f);

        // Направление с учётом разброса
        Vector3 direction = playerCamera.transform.forward;
        direction += Random.insideUnitSphere * spread;
        direction.Normalize();

        // Raycast
        if (Physics.Raycast(playerCamera.transform.position, direction, out RaycastHit hit, range, enemyLayer))
        {
            EnemyTarget enemy = hit.collider.GetComponent<EnemyTarget>();
            if (enemy != null) enemy.TakeDamage(damage);
            else Debug.Log("Hit: " + hit.collider.name);
        }

        // Отладочный луч (виден в Scene View 1 сек)
        Debug.DrawRay(playerCamera.transform.position, direction * range, Color.red, 1f);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        if (reloadSound != null) reloadSound.Play();
        yield return new WaitForSeconds(reloadTime);

        int neededAmmo = maxAmmo - currentAmmo;
        int ammoToAdd = Mathf.Min(neededAmmo, totalAmmo);
        currentAmmo += ammoToAdd;
        totalAmmo -= ammoToAdd;

        isReloading = false;
        UpdateAmmoUI();
    }

    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null) ammoText.text = $"{currentAmmo}/{totalAmmo}";
    }

    public float GetCurrentRecoil() => currentRecoil;
}