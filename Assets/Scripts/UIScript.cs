using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [Header("Jump Markers")]
    public Color jumpFull;
    public Color jumpEmpty;
    public List<Image> jumps = new List<Image>();
    [Space(5)]

    [Header("References")]
    public PlayerScript ps;

    public void updateUI()
    {
        for(int i = 0; i < (jumps.Count); i++)
        {
            if(i + 1 > ps.jumps)
            {
                jumps[i].color = jumpEmpty;
            }

            else
            {
                jumps[i].color = jumpFull;
            }
        }
    }
}
