using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    [Header("Charge Markers")]
    public Color chargeFull;
    public Color chargeEmpty;
    public List<Image> charges = new List<Image>();
    public TMP_Text healthText;
    public Image healthBar;
    [Space(5)]

    [Header("References")]
    public PlayerScript ps;

    public void updateUI()
    {
        for(int i = 0; i < (charges.Count); i++)
        {
            if(i + 1 > ps.charges)
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
