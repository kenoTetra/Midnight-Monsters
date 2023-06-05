using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour, IDamagable
{
    public GameObject hitNumber;

    public void Damage(float damage)
    {
        GameObject curHitNumber = Instantiate(hitNumber, transform.position, Quaternion.identity);
        curHitNumber.GetComponent<HitNumberScript>().updateDamageText(damage);
    }
}
