using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionScript : MonoBehaviour
{
    [SerializeField] private Animator credits;

    public void triggerCredits()
    {
        credits.SetTrigger("Spawn");
    }

    public void untriggerCredits()
    {
        credits.SetTrigger("Out");
    }
}
