using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBulletScript : MonoBehaviour
{
    [Header("Damage Values")]
    public float bulletDamage = 5f;

    // References
    PlayerScript ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();

        // Destroy the bullet after 10 seconds
        Destroy(gameObject, 10f);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            Start();   
            ps.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }

        else if(col.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
}
