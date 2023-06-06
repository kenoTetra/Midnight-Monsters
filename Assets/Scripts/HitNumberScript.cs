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
    [HideInInspector] public bool critHit;
    [HideInInspector] public Color damageColor;
    public Color normalColor = Color.red;
    public Color critColor = Color.yellow;
    [Space(5)]

    [Header("Dynamic Sizing")]
    public float fixedSize = 0.0025f;
    private Vector3 initScale;

    // Start is called before the first frame update
    void Start()
    {
        // References
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindWithTag("Player");
        hitNumber = GetComponentInChildren<TMP_Text>();

        // Dynamic sizing
        initScale = transform.localScale;

        // Crit coloring
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
        // If there's a player in the scene, look at them
        if(target != null)
            transform.LookAt(target.transform);

        // Destroy timer
        if(timeToDestroy > destroyCurTime)
        {
            destroyCurTime += Time.deltaTime;
            hitNumber.color = new Color (damageColor.r, damageColor.g, damageColor.b, (timeToDestroy / destroyCurTime) - 1f);
        }

        else
            Destroy(this.gameObject);

        // Dynamic sizing
        float distance = (target.transform.position - transform.position).magnitude;
        float size = distance * fixedSize / 12.5f;
        transform.localScale = Vector3.one * size;
    }

    // The scrunkly (color fade)
    public void updateDamageText(float damage, bool crit, float critHitMult)
    {
        critHit = crit;
        Start();
        Update();

        if(crit)
            hitNumber.text = "-" + (damage * critHitMult).ToString();

        else
            hitNumber.text = "-" + damage.ToString();
    }
}
