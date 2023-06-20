using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketExplode : MonoBehaviour
{
    [SerializeField] private GunData gunData;
    [SerializeField] private GameObject explosionParticles;
    [SerializeField] private float explosionDistance;
    [SerializeField] private float blowBackDistance;
    ProjectileGlobalCollision projData;
    bool hasExploded;

    // Start is called before the first frame update
    void Start()
    {
        projData = GetComponent<ProjectileGlobalCollision>();
    }

    // Update is called once per frame
    void Update()
    {
        if(projData.killProjectile && !hasExploded)
        {
            // Display particles
            Instantiate(explosionParticles, transform.position, Quaternion.identity);

            // Find all game objects around explosion.
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionDistance);

            foreach(Collider cur_obj in colliders)
            {
                // Blow back everything.              
                // thank u cj for my life
                float distanceRad = 1 - (Vector3.Distance(transform.position, cur_obj.transform.position)/blowBackDistance);

                if(cur_obj.GetComponent<Rigidbody>())
                    cur_obj.GetComponent<Rigidbody>().AddExplosionForce(blowBackDistance * distanceRad, transform.position, explosionDistance, 1.25f * distanceRad, ForceMode.Impulse);

                // Damage them if IDamagable/Player4
                if(cur_obj.transform.gameObject.tag != "CritSpot")
                {
                    cur_obj.GetComponent<IDamagable>()?.Damage(gunData.damage * distanceRad, false, gunData.critHitMult, gunData.gunName);
                    
                    cur_obj.GetComponent<PlayerScript>()?.TakeDamage(gunData.damage * distanceRad * .166f);
                }

                Debug.Log("Object hit: " + cur_obj.transform.gameObject);
                Debug.Log("Position in percent: " + distanceRad);
                Debug.Log("Position not in percent: " + Vector3.Distance(transform.position, cur_obj.transform.position));
            }

            // Stop recalls.
            hasExploded = true;
        }
    }
}
