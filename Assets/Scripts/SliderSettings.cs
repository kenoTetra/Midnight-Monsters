using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SliderSettings : MonoBehaviour
{
    private Slider slider;
    private TMP_Text text;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string mixerName;

    void Start()
    {
        slider = GetComponent<Slider>();
        text = transform.Find("Value").GetComponent<TMP_Text>();
    }

    public void changeVolume(float volume)
    {
        text.text = (1-(volume/-80)).ToString(".00");
        text.text = Regex.Replace(text.text, "[^0-9A-Za-z _-]", "");
        text.text = text.text + "%";
        mixer.SetFloat(mixerName, volume);
    }
}
