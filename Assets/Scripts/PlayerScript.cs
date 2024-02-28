using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100;
    public float health = 100;

    [Header("Checkpoints")]
    public Vector3 startPoint;
    public Vector3 checkpoint;
    [Space(5)]

    [Header("Weapons")]
    public List<GameObject> weaponList = new List<GameObject>();
    [Space(5)]

    [Header("Audio")]
    [SerializeField] private AudioClip hurtSound;
    [Space(5)]

    [Header("Hurt VFX")]    
    [SerializeField] private float fxMaxTime;
    // Ramp time is included in max time. Keep this in mind.
    [SerializeField] private float fxRampTime;
    [SerializeField] private float colorIntensity = 2.11f;
    [SerializeField] private float colorShift = 1.47f;
    [SerializeField] private float vramShift = 3.55f;
    float fxTimer;
    bool fxDisplay;

    // References
    [SerializeField] private UIScript ui;
    BasicPlayerMovement pm;
    AudioSource aud;
    ShaderEffect_BleedingColors bleedingColors;
    ShaderEffect_CorruptedVram corruptedVram;
    Animator anim;
    
    void Start()
    {
        // Shaders
        bleedingColors = GetComponentInChildren<ShaderEffect_BleedingColors>();
        corruptedVram = GetComponentInChildren<ShaderEffect_CorruptedVram>();

        // Ui
        ui = GetComponentInChildren<UIScript>();

        // Other references
        pm = GetComponent<BasicPlayerMovement>();
        aud = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        startPoint = transform.position;

        //startYScale = playerObj.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(!pm.pauseHandler.paused)
        {
            changeWeapons();

            // death check
            if(health <= 0)
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }

            if(fxDisplay)
            {
                ShowVFX();
            }
        }

        // Weapon cooldowns when not equipped
        foreach(GameObject weapon in weaponList)
        {
            weapon.GetComponent<GunScript>().lastShotTime += Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Hazard")
        {
            if(checkpoint != null)
                transform.position = checkpoint;

            else
                transform.position = startPoint;

            health -= 25;
            ui.updateUI();
        }
    }

    void changeWeapons()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            newWeapon(1);
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            newWeapon(2);
        }

        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            newWeapon(3);
        }

        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            newWeapon(4);
        }
    }

    void newWeapon(int alpha)
    {
        foreach(GameObject weapon in weaponList)
        {
            weapon.SetActive(false);
        }

        weaponList[alpha-1].SetActive(true);
        anim.SetTrigger("Select New");

        ui.FadeOtherWeapons(alpha, weaponList[alpha-1].GetComponent<GunScript>().gunData);
    }

    public void TakeDamage(float damage)
    {
        if(damage >= maxHealth && maxHealth == health)
        {
            ui.prevHealth = health;

            health = 1;

            fxDisplay = true;
            PlaySound(hurtSound);
        }

        else
        {
            health -= damage;
            health = Mathf.Clamp(health, 0f, maxHealth);

            ui.prevHealth = health + damage;

            fxDisplay= true;
            PlaySound(hurtSound);
        }

        ui.updateUI();
        ui.lerpBad = true;
    }

    public void GainHealth(float healthGained)
    {
        health += healthGained;
        health = Mathf.Clamp(health, 0f, maxHealth);

        ui.updateUI();
        ui.lerpGood = true;
    }

    public void ShowVFX()
    {
        // Run a timer.
        fxTimer += Time.deltaTime;

        // For the ramp, make the visuals appear gradually.
        if(fxTimer < fxRampTime)
        {
            // Enable VFX display.
            bleedingColors.enabled = true;
            corruptedVram.enabled = true;

            float rampPerc = (fxTimer/fxRampTime);

            bleedingColors.intensity = colorIntensity * rampPerc;
            bleedingColors.shift = colorShift * rampPerc;
            corruptedVram.shift = vramShift * rampPerc;
        }

        // After the ramp, scale the effect back down.
        else if(fxTimer < fxMaxTime)
        {
            // Get the float as a percentage ramping downwards.
            float totalPerc = 1-((fxTimer-fxRampTime)/(fxMaxTime-fxRampTime));

            bleedingColors.intensity = colorIntensity * totalPerc;
            bleedingColors.shift = colorShift * totalPerc;
            corruptedVram.shift = vramShift * totalPerc;
        }
        
        // When the display time is up...
        else
        {
            // Disable VFX display.
            bleedingColors.enabled = false;
            corruptedVram.enabled = false;

            // Set intensities to zero.
            bleedingColors.intensity = 0f;
            bleedingColors.shift = 0f;
            corruptedVram.shift = 0f;

            // Reset timer, and stop displaying VFX.
            fxTimer = 0f;
            fxDisplay = false;
        }
    }

    public void PlaySound(AudioClip sound)
    {
        aud.PlayOneShot(sound, .5f);
    }
}
