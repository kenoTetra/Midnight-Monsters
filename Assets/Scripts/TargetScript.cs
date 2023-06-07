using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour, IDamagable
{
    public int health = 300;
    public GameObject hitSpawn;

    public void Damage(float damage, bool crit, float critHitMult, string gunName)
    {
        // Damage target
        if(crit)
            health -= (int)(damage * critHitMult);
        
        else
            health -= (int)damage;
        
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }

        // Hit numbers
        IDamagable.spawnHitNumber(damage, crit, critHitMult, hitSpawn.transform.position);
    }
}
