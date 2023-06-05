using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HitNumberScript : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    private GameObject target;
    private TMP_Text hitNumber;

    [Header("Damage Number Data")]
    public float timeToDestroy = 2f;
    private float destroyCurTime;
    public float randomBounce = 1f;
    public Color damageColor;
    public Color normalColor = Color.red;
    public Color critColor = Color.yellow;
    public bool critHit;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindWithTag("Player");
        hitNumber = GetComponentInChildren<TMP_Text>();

        if(critHit)
            damageColor = critColor;
        
        else
            damageColor = normalColor;

        // Shoots it off in a random direction kinda cute like :3
        rb.AddForce(new Vector3(Random.Range(-randomBounce, randomBounce), 1.5f, Random.Range(-randomBounce, randomBounce)), ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
            transform.LookAt(target.transform);

        if(timeToDestroy > destroyCurTime)
        {
            destroyCurTime += Time.deltaTime;
            hitNumber.color = new Color (damageColor.r, damageColor.g, damageColor.b, (timeToDestroy / destroyCurTime) - 1f);
        }

        else
        {
            Destroy(this.gameObject);
        }
    }

    public void updateDamageText(float damage, bool crit, float critHitMult)
    {
        critHit = crit;
        Start();

        if(crit)
            hitNumber.text = "-" + (damage * critHitMult).ToString();

        else
            hitNumber.text = "-" + damage.ToString();
    }
}
