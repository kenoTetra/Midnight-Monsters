using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGlobalCollision : MonoBehaviour
{
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

    void OnCollision(Collision col)
    {
        // Stop rendering everything but allow time for logic
        mesh.enabled = false;

        if(trail != null)
            trail.emitting = false;

        rb.detectCollisions = false;

        // Destroy self after logic runs.
        Destroy(gameObject, 1f);
    }
}
