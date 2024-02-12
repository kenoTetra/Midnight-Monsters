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
    [SerializeField] private Image gunDisplay;
    [SerializeField] private List<Image> weaponDots = new List<Image>();
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
        
        // Update the healthbar.
        healthText.text = Mathf.CeilToInt(ps.health).ToString();
        float hpToSet = (float)ps.health/(float)ps.maxHealth;

        // Round to the nearest .1
        healthBar.fillAmount = (float)Mathf.Round(hpToSet * 10.0f) * 0.1f;
    }

    void Update()
    {
        if(lerpGood)
        {
            goodLerpTime += Time.deltaTime;
            float hpToSet = (float)ps.health/(float)ps.maxHealth;

            healthBar.color = Color.Lerp(healthBarEffectColor, healthColor, goodLerpTime/.5f);
            healthEffectBar.fillAmount = (float)Mathf.Round(hpToSet * 10.0f) * 0.1f;
            
            if(goodLerpTime > .5f)
                lerpGood = false;
        }

        else if(lerpBad)
        {
            badLerpTime += Time.deltaTime;

            /// Get hp to set
            float hpToSet = (float)ps.health/(float)ps.maxHealth;

            // Lerp to it over the course of 1s
            var badFillAmm = Mathf.Lerp(prevHealth/ps.maxHealth, (float)Mathf.Round(hpToSet * 10.0f) * 0.1f, badLerpTime/1f);
            healthEffectBar.fillAmount = badFillAmm;
            
            if(badLerpTime > 1.0f)
                lerpBad = false;
        }

        else
        {
            goodLerpTime = 0f;
            badLerpTime = 0f;
        }
    }

    public void FadeOtherWeapons(int weaponActive, GunData gunData)
    {
        for(int i = 0; i < weaponDots.Count; i++)
        {
            weaponDots[i].color = inactiveWeaponColor;

        }

        weaponDots[weaponActive-1].color = activeWeaponColor;

        gunDisplay.sprite = gunData.UIImage;
    }
}
