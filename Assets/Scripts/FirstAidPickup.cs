using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidPickup : MonoBehaviour
{
    [Header("Health Pickup Data")]
    [SerializeField] private int healthGained = 25;
    [SerializeField] private float randomBounce = 5f;
    [SerializeField] private LayerMask groundLayer;
    bool rotationStarted;

    // References
    PlayerScript ps;
    Rigidbody rb;
    Animator animator;

    void Start()
    {
        ps = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.AddForce(new Vector3(Random.Range(-randomBounce, randomBounce), 2.5f, Random.Range(-randomBounce, randomBounce)), ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player" && ps.health != ps.maxHealth)
        {
            ps.GainHealth(healthGained);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(Physics.Raycast(transform.position, Vector3.down, .25f, groundLayer) && !rotationStarted)
        {
            animator.SetBool("Rotate", true);
            rotationStarted = true;
        }
    }
}
