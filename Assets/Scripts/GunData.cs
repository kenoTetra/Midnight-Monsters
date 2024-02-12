using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName="Gun", menuName="Weapon/Gun")]

public class GunData : ScriptableObject
{
    public enum GunType {
        Automatic,
        SingleShot,
        Projectile
    }

    [Header("Stats")]
    public string gunName;
    public float damage;
    public float critHitMult;
    public float maxDistance;
    [Space(5)]

    [Header("Weapon Type Data")]
    public GunType weaponType;
    public GameObject muzzleParticles;
    public GameObject fireParticles;
    public int automaticFireRate;
    public float projectileSpeed;
    public GameObject projectilePrefab;
    public float singleShotDelay;
    [Space(5)]

    [Header("Weapon Sounds")]
    public List<AudioClip> fireSounds = new List<AudioClip>();
    [Space(5)]

    [Header("Visuals")]
    public Sprite UIImage;
}
