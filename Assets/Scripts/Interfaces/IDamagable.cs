using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public void Damage(float damage, bool crit, float critHitMult, string gunName);

    public static void spawnHitNumber(float damage, bool crit, float critHitMult, Vector3 spawnLoc)
    {
        GameObject curHitNumber = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Gameplay/HitCanvas"), spawnLoc, Quaternion.identity);
        curHitNumber.GetComponent<HitNumberScript>().updateDamageText(damage, crit, critHitMult);
    }
}
