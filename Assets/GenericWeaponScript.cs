using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericWeaponScript : MonoBehaviour
{
    [Header("Stats")]
    public float damage;
    public float critMult;
    public float delay;
    public float spread;
    public float bulletsInShot;
    public bool autoFire;
    private bool shooting;
    private bool readyToFire;
    [Space(5)]

    [Header("References")]
    public Camera cam;
    public Transform shotLocation;
    public RaycastHit rayHit;
    public LayerMask Targets;
    public GameObject bulletHole;
    public GameObject muzzleFlash;
    public GameObject barrel;

    void Start()
    {
        readyToFire = true;
    }

    void Update()
    {
        input();
    }

    void input()
    {
        if(autoFire && Input.GetAxisRaw("Fire1") > 0)
        {
            shooting = true;
        }

        else if(!autoFire)
            shooting = Input.GetButtonDown("Fire1");

        if(readyToFire && shooting)
        {
            shoot();
        }
    }

    void shoot()
    {
        readyToFire = false;

        // Spread
        float spreadX = Random.Range(-spread, spread);
        float spreadY = Random.Range(-spread, spread);
        Vector3 spreadDir = cam.transform.forward + new Vector3(spreadX, spreadY, 0);

        // Hitting Target
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, 100f, Targets))
        {
            Debug.Log("Hit a target! Dealt " + damage + " damage.");

            if(rayHit.collider.tag == "Target")
            {
                rayHit.collider.GetComponent<TargetScript>().takeDamage(damage, critMult, false);
            }

            else if(rayHit.collider.tag == "TargetCrit")
            {
                rayHit.collider.GetComponent<TargetScript>().takeDamage(damage, critMult, true);
            }
        }

        // Graphics
        Instantiate(bulletHole, rayHit.point, Quaternion.Euler(0,180,0));
        Instantiate(muzzleFlash, barrel.transform.position, Quaternion.identity);

        Invoke("resetShot", delay);
    }

    void resetShot()
    {
        readyToFire = true;
    }
}
