using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Gun", menuName="Weapon/Gun")]

public class GunData : ScriptableObject
{
    [Header("Stats")]
    public new string name;
    public float damage;
    public float maxDistance;
    public bool automatic;
    public int automaticFireRate;
    public float singleShotDelay;
}
