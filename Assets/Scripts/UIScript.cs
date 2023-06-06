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
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;
    [Space(5)]

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
}
