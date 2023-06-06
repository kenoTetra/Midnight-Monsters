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

    void Start()
    {
        animator = GameObject.FindWithTag("Player").GetComponent<Animator>();
        audioSource = GameObject.FindWithTag("GunSound").GetComponent<AudioSource>();
        lastShotTime = gunData.singleShotDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if(gunData.automatic && Input.GetAxisRaw("Fire1") > 0)
        {
            Shoot();
        }

        else if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        lastShotTime += Time.deltaTime;
        Debug.DrawRay(bulletPos.position, bulletPos.forward, Color.red);
    }

    // Single shot delay OR shots fired every second divided by a second (ala, 600 rpm is 10rps, which is .1s between shots)
    private bool CanShoot() => lastShotTime > gunData.singleShotDelay && !gunData.automatic || lastShotTime > 1f / (gunData.automaticFireRate / 60f) && gunData.automatic;

    void Shoot()
    {
        if(CanShoot())
        {
            if(Physics.Raycast(bulletPos.position, bulletPos.forward, out RaycastHit hitInfo, gunData.maxDistance))
            {
                if(hitInfo.transform.gameObject.tag == "Crit")
                {
                    critHit = true;
                    IDamagable damageable = hitInfo.transform.GetComponent<CritSpotScript>().target;
                    damageable?.Damage(gunData.damage, critHit, gunData.critHitMult);
                    Debug.Log("Hit a critical!");
                }
                
                else
                {
                    critHit = false;
                    IDamagable damageable = hitInfo.transform.GetComponent<IDamagable>();
                    damageable?.Damage(gunData.damage, critHit, gunData.critHitMult);
                }
                
            }

            lastShotTime = 0;
            OnGunShot();
        }
    }

    private void OnGunShot()
    {
        animator.SetBool("Fire", true);

        if(gunData.name != "Melee" || !audioSource.isPlaying)
            audioSource.PlayOneShot(gunData.fireSounds[Random.Range(0, gunData.fireSounds.Count - 1)], .5f);
    }
}
