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
    private float randDir() => Random.Range(-randomBounce, randomBounce);
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
        damageColor = critHit ? critColor : normalColor;

        // Shoots off damage number in a random direction.
        rb.AddForce(new Vector3(randDir(), 1.5f, randDir()), ForceMode.Impulse);

        // Destroy timer
        Destroy(this.gameObject, timeToDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        // If there's a player in the scene, look at them
        if(target != null)
            transform.LookAt(target.transform);

        // Fade text with countdown of destroy timer.
        if(timeToDestroy > destroyCurTime)
        {
            destroyCurTime += Time.deltaTime;
            hitNumber.color = new Color (damageColor.r, damageColor.g, damageColor.b, (timeToDestroy / destroyCurTime) - 1f);
        }

        // Dynamic sizing
        float distance = (target.transform.position - transform.position).magnitude;
        float size = distance * fixedSize / 12.5f;
        transform.localScale = Vector3.one * size;
    }

    // The scrunkly (color fade)
    public void updateDamageText(float damage, bool crit, float critHitMult)
    {
        // Pass the crit outside of the referenced script.
        critHit = crit;

        // Make sure that the other functions fire first
        Start();
        Update();

        // Pass damage text
        if(crit)
            hitNumber.text = "-" + System.Math.Round((damage * critHitMult),2).ToString();

        else
            hitNumber.text = "-" + System.Math.Round(damage,2).ToString();
    }
}
