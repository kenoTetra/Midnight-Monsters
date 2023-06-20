using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGlobalCollision : MonoBehaviour
{
    public bool killProjectile = true;
    public float projectileLifetime;
    float timer;
    MeshRenderer mesh;
    TrailRenderer trail;
    Rigidbody rb;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();

        if(GetComponent<TrailRenderer>() != null)
            trail = GetComponent<TrailRenderer>();

        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > projectileLifetime)
        {
            ProjectileDestroy();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        ProjectileDestroy();
    }

    void ProjectileDestroy()
    {
        // Start other script logic
        killProjectile = true;

        // Stop rendering everything but allow time for logic
        mesh.enabled = false;

        if(trail != null)
            trail.emitting = false;

        rb.detectCollisions = false;

        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            if(child.gameObject != this.gameObject)
                child.gameObject.SetActive(false);
        }

        // Destroy self after logic runs.
        Destroy(gameObject, 1f);
    }
}
