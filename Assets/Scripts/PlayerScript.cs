using System.Collections;
using System.Collections.Generic;
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

    // References
    UIScript ui;
    BasicPlayerMovement pm;
    
    void Start()
    {
        ui = GetComponentInChildren<UIScript>();
        pm = GetComponent<BasicPlayerMovement>();
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
        ui.FadeOtherWeapons(alpha);
    }

    public void TakeDamage(float damage)
    {
        if(damage >= maxHealth)
        {
            ui.prevHealth = health;

            health = 1;
        }

        else
        {
            health -= damage;
            health = Mathf.Clamp(health, 0f, maxHealth);

            ui.prevHealth = health + damage;
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
}
