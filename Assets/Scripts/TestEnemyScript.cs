using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyScript : MonoBehaviour, IDamagable
{
    [Header("Bullet Data")]
    [SerializeField] private GameObject bulletSpawnPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 25f;
    float timeSinceLastShot;
    [SerializeField] private float timeBetweenShots = .5f;

    [Header("Enemy Data")]
    public float health = 100f;
    [SerializeField] private GameObject healthPickup;
    [SerializeField] private float dropRate;
    bool attacking;

    void Attack()
    {
        Debug.Log("Enemy begins attacking");

        attacking = true;
    }

    void StopAttack()
    {
        attacking = false;

        Debug.Log("Enemy stops attacking");
    }

    void FixedUpdate()
    {
        // Rotate the GameObject every frame so it keeps facing the target
        transform.LookAt(GameObject.FindWithTag("Player").transform);
        bulletSpawnPoint.transform.LookAt(GameObject.FindWithTag("Player").transform);

        if(attacking)
        {
            if(timeSinceLastShot > timeBetweenShots)
            {
                // Instantiate a bullet at the position of the GameObject
                // which the script is attached to
                var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, Quaternion.identity);

                // Get the Rigidbody component of the instantiated bullet
                var bulletRigidbody = bullet.GetComponent<Rigidbody>();

                // Add force to the bullet in the direction the GameObject is facing
                bulletRigidbody.AddForce(bulletSpawnPoint.transform.forward * bulletSpeed, ForceMode.Impulse);

                timeSinceLastShot = 0f;
            }

            else
            {
                timeSinceLastShot += Time.deltaTime;
            }
        }

        else
        {
            timeSinceLastShot = 0f;
        }
    }

    void IDamagable.Damage(float damage, bool crit, float critHitMult, string gunName)
    {
        // Calculate the actual damage to be dealt
        var actual_damage = damage * (crit ? critHitMult : 1f);

        // Deal damage to the enemy
        health -= actual_damage;

        if(gunName != "Hazard")
            IDamagable.spawnHitNumber(damage, crit, critHitMult, bulletSpawnPoint.transform.position);

        // Log the damage dealt
        Debug.Log("Enemy takes " + actual_damage + " damage");

        if(health <= 0)
        {
            if(gunName == "Melee" && Random.Range(0f, 1f) > 1f - dropRate)
                Instantiate(healthPickup, bulletSpawnPoint.transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Hazard")
        {
            Destroy(gameObject);
        }
    }
}
