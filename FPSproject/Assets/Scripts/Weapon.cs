using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;
    public bool isADS;
    public int weaponDamage;
    
    [Header("Shooting")]
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    [Header("Burst")]
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    [Header("Spread")]
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;
    
    public GameObject muzzleEffect;
    internal Animator animator;

    [Header("Loading")]
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    public enum WeaponModel
    {
        M1911,
        AK74
    }

    public WeaponModel thisWeaponModel;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;
    }

    public ShootingMode currentShootingMode;

    void Update()
    {
        if (isActiveWeapon)
        {
            gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
                foreach (Transform secondChild in child.transform)
                {
                    secondChild.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                EnterADS();
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                ExitADS();
            }

            GetComponent<Outline>().enabled = false;
            
            if (bulletsLeft == 0 && isShooting) SoundManager.Instance.emptyMagazineSound.Play();
    
            if (currentShootingMode == ShootingMode.Auto)
            {
                // Holding Down Left Mouse Button
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if(currentShootingMode == ShootingMode.Burst || 
                currentShootingMode == ShootingMode.Single)
            {
                // Clicking Left Mouse Button
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }
    
            // Shoot
            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }
    
            // Reload
            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false 
                && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
                foreach (Transform secondChild in child.transform)
                {
                    secondChild.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
        }
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (isADS)
        {
            animator.SetTrigger("ADS_RECOIL");
        }
        else
        {
            animator.SetTrigger("RECOIL");
        }

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        bullet.GetComponent<Bullet>().bulletDamage = weaponDamage;

        // Pointing the bullet to face the shooting direction
        bullet.transform.forward = shootingDirection;

        // Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // Destroy the bullet after some time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if(allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst Mode
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void EnterADS()
    {
        animator.SetTrigger("enterADS");
        isADS = true;
        HUDManager.Instance.middleDot.SetActive(false);
        spreadIntensity = adsSpreadIntensity;
    }

    private void ExitADS()
    {
        animator.SetTrigger("exitADS");
        isADS = false;
        HUDManager.Instance.middleDot.SetActive(true);
        spreadIntensity = hipSpreadIntensity;
    }

    private void Reload()
    {
        isReloading = true;
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        animator.SetTrigger("RELOAD");
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        if (WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > magazineSize)
        {
            bulletsLeft = magazineSize;
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        }
        else
        {
            bulletsLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, thisWeaponModel);
        }

        isReloading = false;
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        // Shooting from the middle of the screen to check where are we pointing at
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // Hitting Something
            targetPoint = hit.point;
        }
        else
        {
            // Shooting at the air
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        
        // Returnig the shooting direction and spread
        return direction + new Vector3(0, y, z);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
       yield return new WaitForSeconds(delay);
       Destroy(bullet);
    }
}
