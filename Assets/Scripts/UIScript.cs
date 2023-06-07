using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    [Header("Charge Markers")]
    [SerializeField] private Color chargeFull;
    [SerializeField] private Color chargeEmpty;
    [SerializeField] private List<Image> charges = new List<Image>();
    [Space(5)]

    [Header("Health Information")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthEffectBar;
    [SerializeField] private Color healthBarEffectColor = Color.green;
    [SerializeField] private Color healthColor = Color.red;
    [HideInInspector] public bool lerpBad;
    [HideInInspector] public bool lerpGood;
    float goodLerpTime;
    float badLerpTime;
    [HideInInspector] public float prevHealth = 100f;
    [Space(5)]

    [Header("Weapon UI")]
    [SerializeField] private List<Image> weapons = new List<Image>();
    [SerializeField] private Color activeWeaponColor;
    [SerializeField] private Color inactiveWeaponColor;

    [Header("References")]
    PlayerCharges pc;
    PlayerScript ps;

    void Start()
    {
        ps = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerCharges>();
    }

    public void updateUI()
    {
        for(int i = 0; i < (charges.Count); i++)
        {
            if(i + 1 > pc.charges)
            {
                charges[i].color = chargeEmpty;
            }

            else
            {
                charges[i].color = chargeFull;
            }
        }
        
        healthText.text = ps.health.ToString();
        healthBar.fillAmount = (float)ps.health/(float)ps.maxHealth;
    }

    void Update()
    {
        if(lerpGood)
        {
            goodLerpTime += Time.deltaTime;

            healthBar.color = Color.Lerp(healthBarEffectColor, healthColor, goodLerpTime/.5f);
            
            if(goodLerpTime > .5f)
                lerpGood = false;
        }

        else if(lerpBad)
        {
            badLerpTime += Time.deltaTime;

            var badFillAmm = Mathf.Lerp(prevHealth/ps.maxHealth, ps.health/ps.maxHealth, badLerpTime/1.5f);
            healthEffectBar.fillAmount = badFillAmm;
            
            if(badLerpTime > 1.5f)
                lerpBad = false;
        }

        else
        {
            goodLerpTime = 0f;
            badLerpTime = 0f;
        }
    }

    public void FadeOtherWeapons(int weaponActive)
    {
        for(int i = 0; i < weapons.Count; i++)
        {
            weapons[i].color = inactiveWeaponColor;
            weapons[i].gameObject.GetComponentInChildren<TMP_Text>().color = inactiveWeaponColor;

        }

        weapons[weaponActive-1].color = activeWeaponColor;
        weapons[weaponActive-1].gameObject.GetComponentInChildren<TMP_Text>().color = activeWeaponColor;
    }
}
