/************************************************
File: PlayerCharges.cs
Author: Tetra

Description: 
    Handles everything to do with the charge
    system.

    This currently includes:
      - Total charges value handling.
      - Jumping
      - Dashing
************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharges : MonoBehaviour
{
    [Header("Charges")]
    public int maxCharges = 3;
    [HideInInspector] public int charges = 3;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 5f;
    public float playerHeight = 2f;
    
    [SerializeField] private float jumpCDMax = .2f;
    float jumpCD;

    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 10f;

    [Header("Sounds")]
    [SerializeField] private List<AudioClip> jumpSounds = new List<AudioClip>();
    [SerializeField] private List<AudioClip> landSounds = new List<AudioClip>();

    [Header("References")]
    BasicPlayerMovement pm;
    PlayerWallrun pr;
    PlayerSliding ps;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<BasicPlayerMovement>();
        pr = GetComponent<PlayerWallrun>();
        ps = GetComponent<PlayerSliding>();
        jumpCD = jumpCDMax;
    }

    // Update is called once per frame
    void Update()
    {
        if(!pr.isWallRunning)
        {
            if(Input.GetButtonDown("Jump"))
                Jumping();

            if(Input.GetButtonDown("Dash"))
                Dashing();

            CheckChargeReset();
        }
    }

    void Jumping()
    {
        // Jumping 
        if(charges > 0)
        {
            charges -= 1;
            jumpCD = 0;
            pm.rb.velocity = new Vector3(pm.rb.velocity.x, 0f, pm.rb.velocity.z);
            pm.rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); 
            ps.sliding = false;
            ps.StopSlide();

            // Play jump sounds
            pm.audioSource.PlayOneShot(jumpSounds[Random.Range(0, landSounds.Count - 1)], 1f);

            // Update jump counter visuals
            pm.ui.updateUI();
        }
    }

    void Dashing()
    {
        if(charges > 0)
        {
            charges -= 1;
            pm.rb.velocity = new Vector3(pm.rb.velocity.x, 0f, pm.rb.velocity.z);

            if(pm.moveDir != Vector3.zero)
                pm.rb.AddForce(pm.moveDir * dashSpeed, ForceMode.Impulse);

            else
                pm.rb.AddForce(Vector3.forward * dashSpeed, ForceMode.Impulse);

            // Play jump sounds
            pm.audioSource.PlayOneShot(jumpSounds[Random.Range(0, landSounds.Count - 1)], 1f);

            // Update jump counter visuals
            pm.ui.updateUI();
        }
    }

    void CheckChargeReset()
    {
        // Reset charges if you're grounded and it's been jumpCDMax since you weren't grounded.
        if(pm.grounded && jumpCD >= jumpCDMax)
        {
            // Audio for landing
            if(!pm.audioSource.isPlaying && charges < maxCharges)
                pm.audioSource.PlayOneShot(landSounds[Random.Range(0, landSounds.Count - 1)], 1f);

            charges = maxCharges;

            // Reset jump count visuals   
            pm.ui.updateUI();
        }

        // Timer for charge reset
        if(jumpCD <= jumpCDMax)
        {
            jumpCD += Time.deltaTime;
        }
    }
}
