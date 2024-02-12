using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSwitch : MonoBehaviour
{
    [SerializeField] private GameObject toToggle;

    public void Toggle(bool state)
    {
        toToggle.SetActive(state);
    }
}
