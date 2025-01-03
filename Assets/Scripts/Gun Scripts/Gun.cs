using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public enum GunMode { SemiAuto, FullyAuto }
    public GunMode mode;

    [SerializeField] float fireRate;
    [SerializeField] Bullet bullet;
    [SerializeField] Transform firePoint;
    [SerializeField] float bulletDamage;
    [SerializeField] float bulletSpeed;
    [SerializeField] AudioClip bulletShotSound;
    [SerializeField] AudioClip reloadSound;


    [Header("Ammo System")]
    [SerializeField] int bulletInMagzine;
    [SerializeField] int totalBullets;
    [SerializeField] int currentBullet;

    private int orignalTotalBullets;
    private int orignalCurrentBullets;

    private float lastFireTime;
    private AudioSource audioSource;
    private Coroutine fullyAutoCoroutine;
    private UiManager uiManager;


    private void Start()
    {
        Init();
    }

    /*private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            FireButtonDown();
        }

        if (Input.GetMouseButtonUp(1))
        {
            FireButtonUp();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }*/

    public void Init() 
    {
        uiManager = UiManager.Instance;
        audioSource = GetComponent<AudioSource>();
        orignalCurrentBullets = currentBullet;
        orignalTotalBullets = totalBullets;
        Reload();
    }


    #region OnButtonPressed
    public void OnPointerDown(PointerEventData eventData)
    {
        FireButtonDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        FireButtonUp();
    }

    public void FireButtonDown()
    {
        if (mode == GunMode.FullyAuto && fullyAutoCoroutine == null)
        {
            fullyAutoCoroutine = StartCoroutine(FullyAutoFire());
        }

    }
    public void FireButtonUp()
    {
        if (mode == GunMode.SemiAuto)
        {
            Shoot();
        }
        else if (mode == GunMode.FullyAuto && fullyAutoCoroutine != null)
        {
            StopCoroutine(fullyAutoCoroutine);
            fullyAutoCoroutine = null;
        }
        if(currentBullet <= 0)
            Reload();
    }
    #endregion

    #region ShootFunctions
    public void Shoot()
    {
        if (currentBullet > 0 && Time.time - lastFireTime >= fireRate)
        {
            Bullet bulletPrefab = Instantiate(bullet, firePoint.position, firePoint.rotation);
            bulletPrefab.OnBulletSpawn(bulletSpeed, bulletDamage);
            audioSource.PlayOneShot(bulletShotSound);
            currentBullet--;
            ChangeBulletCount(currentBullet, totalBullets);
            lastFireTime = Time.time;
        }
    }

    private IEnumerator FullyAutoFire()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(fireRate); 
        }
    }
    #endregion

    public void Reload()
    {
        if (currentBullet == bulletInMagzine)
        {
            ChangeBulletCount(currentBullet,totalBullets);
            return;
        }
        audioSource.PlayOneShot(reloadSound);

        int bulletsNeeded = bulletInMagzine - currentBullet;
        if (totalBullets >= bulletsNeeded)
        {
            currentBullet += bulletsNeeded;
            totalBullets -= bulletsNeeded;
        }
        else
        {
            currentBullet += totalBullets;
            totalBullets = 0;
        }
        ChangeBulletCount(currentBullet, totalBullets);
    }

    public void GetMoreBullets(int amount)
    {
        totalBullets += amount;
        Reload();
    }

    public void ChangeBulletCount(int cb, int tb)
    {
        uiManager.UpdateBulletUi(cb, tb);
    }

    public void ResetMyBulletsForFirstWave()
    {
        currentBullet = orignalCurrentBullets;
        totalBullets = orignalTotalBullets;
        Reload();
    }

}
