using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyGunScript : MonoBehaviour
{
    // The GameObject from which the bullet will be instantiated
    [SerializeField]
    private GameObject bulletSpawnPoint;

    // The bullet prefab
    [SerializeField]
    private GameObject bulletPrefab;

    // Whether the enemy is attacking
    private bool attacking = false;

    void Attack()
    {
        Debug.Log("Enemy begins attacking");

        // Instantiate a bullet at the position of the GameObject
        // which the script is attached to
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, Quaternion.identity);

        // Get the Rigidbody component of the instantiated bullet
        var bulletRigidbody = bullet.GetComponent<Rigidbody>();

        // Add force to the bullet in the direction the GameObject is facing
        bulletRigidbody.AddForce(transform.forward * 1000f);
    }

    void StopAttack()
    {
        Debug.Log("Enemy stops attacking");
    }

    void FixedUpdate()
    {
        // Rotate the GameObject every frame so it keeps facing the target
        transform.LookAt(GameObject.FindWithTag("Player").transform);
    }
}
