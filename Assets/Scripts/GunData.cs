using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Gun", menuName="Weapon/Gun")]

public class GunData : ScriptableObject
{
    [Header("Stats")]
    public string gunName;
    public float damage;
    public float critHitMult;
    public float maxDistance;
    [Space(5)]

    [Header("Weapon Type")]
    public bool automatic;
    public int automaticFireRate;
    public float singleShotDelay;
    [Space(5)]

    [Header("Weapon Sounds")]
    public List<AudioClip> fireSounds = new List<AudioClip>();
}
