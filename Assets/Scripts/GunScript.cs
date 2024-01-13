using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [Header("References")]
    public GunData gunData;
    public Transform muzzle;
    public Transform bulletPos;
    [Space(5)]

    [Header("Shooting Data")]
    public float lastShotTime;
    public bool critHit;

    // References
    Animator animator;
    AudioSource audioSource;
    Transform p_transform;
    Transform cam_transform;

    void Start()
    {
        animator = GameObject.FindWithTag("Player").GetComponent<Animator>();
        p_transform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        cam_transform = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
        audioSource = GameObject.FindWithTag("GunSound").GetComponent<AudioSource>();
        lastShotTime = gunData.singleShotDelay;
    }

    // Update is called once per frame
    void Update()
    {
        // Automatic guns (fire while held)
        if(gunData.weaponType == GunData.GunType.Automatic && Input.GetAxisRaw("Fire1") > 0)
        {
            Shoot();
        }

        // Single shot guns (fire on tap)
        else if(gunData.weaponType == GunData.GunType.SingleShot && Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        // Single shot projectile weapons (fire on tap)
        else if(gunData.weaponType == GunData.GunType.Projectile && Input.GetButtonDown("Fire1"))
        {
           ProjectileShoot();
        }

        // Reset the time between the shots.
        lastShotTime += Time.deltaTime;
        Debug.DrawRay(bulletPos.position, bulletPos.forward, Color.red);
    }

    // Single shot delay OR shots fired every second divided by a second (ala, 600 rpm is 10rps, which is .1s between shots)
    private bool CanShoot() => lastShotTime > gunData.singleShotDelay && gunData.weaponType == GunData.GunType.SingleShot
     || lastShotTime > 1f / (gunData.automaticFireRate / 60f) && gunData.weaponType == GunData.GunType.Automatic
     || lastShotTime > gunData.singleShotDelay && gunData.weaponType == GunData.GunType.Projectile;

    private void Shoot()
    {
        if(CanShoot())
        {
            if(Physics.Raycast(bulletPos.position, bulletPos.forward, out RaycastHit hitInfo, gunData.maxDistance))
            {
                if(hitInfo.transform.gameObject.tag == "Crit")
                {
                    critHit = true;

                    if(hitInfo.transform.GetComponent<CritSpotScript>().target != null)
                    {
                        IDamagable damageable = hitInfo.transform.GetComponent<CritSpotScript>().target;
                        damageable?.Damage(gunData.damage, critHit, gunData.critHitMult, gunData.gunName);
                    }

                    else if(hitInfo.transform.GetComponent<CritSpotScript>().enemyScript != null)
                    {
                        IDamagable damageable = hitInfo.transform.GetComponent<CritSpotScript>().enemyScript;
                        damageable?.Damage(gunData.damage, critHit, gunData.critHitMult, gunData.gunName);
                    }
                    
                    Debug.Log("Hit a critical!");
                }
                
                else
                {
                    critHit = false;
                    IDamagable damageable = hitInfo.transform.GetComponent<IDamagable>();
                    damageable?.Damage(gunData.damage, critHit, gunData.critHitMult, gunData.gunName);
                }
                
            }

            lastShotTime = 0;
            OnGunShot();
        }
    }

    private void OnGunShot()
    {
        animator.SetBool("Fire", true);
        SpawnParticles();

        if(gunData.name != "Melee" || !audioSource.isPlaying)
            audioSource.PlayOneShot(gunData.fireSounds[Random.Range(0, gunData.fireSounds.Count - 1)], .5f);
    }

    void ProjectileShoot()
    {
        if(CanShoot())
        {
            if(gunData.projectilePrefab != null)
            {
                GameObject projectileShot = Instantiate(gunData.projectilePrefab, transform.position, transform.rotation * Quaternion.Euler(90, 0, 0));
                projectileShot.GetComponent<Rigidbody>().AddForce(transform.forward * gunData.projectileSpeed, ForceMode.Impulse);
                SpawnParticles();
            }
        }
    }

    void SpawnParticles()
    {
        if(gunData.muzzleParticles != null)
        {
            GameObject particles = Instantiate(gunData.muzzleParticles, muzzle.transform.position, gunData.muzzleParticles.transform.rotation, muzzle.transform);
            Destroy(particles, 1.5f);
        }

        if(gunData.fireParticles != null)
        {
            GameObject particles = Instantiate(gunData.fireParticles, muzzle.transform.position, gunData.fireParticles.transform.rotation * cam_transform.rotation /* p_transform.rotation */);
            Destroy(particles, 1.5f);
        }
    }
}
