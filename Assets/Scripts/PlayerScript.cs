using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    [Header("Input")]
    private float moveX;
    private float moveY;
    private Vector3 moveDir;
    private bool moveJump;
    private float moveJumpHold;
    private Transform orientation;

    [Header("Basic Movement")]
    public float speed = 10f;
    [Space(5)]
    
    [Header("Jumping/Grounded")]
    public float groundDrag;
    public float playerHeight;
    private bool grounded;
    private float jumpCD = .2f;
    public float jumpCDMax = .2f;
    public LayerMask groundLayer;
    public float jumpForce = 5f;
    public int jumps = 2;
    public int maxJumps = 2;
    public float fallShakeMagnitude = .4f;
    public float speedTillShake = -2f;
    [Space(5)]

    [Header("Wallrunning")]
    public float wallDistance = .5f;
    public float minimumJumpHeight = 1.5f;
    private bool wallLeft;
    private bool canWallrun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    [Space(5)]
    private RaycastHit slopeHit;

    [Header("Health")]
    public int maxHealth = 100;
    public int health = 100;

    [Header("Checkpoints")]
    public Vector3 startPoint;
    public Vector3 checkpoint;
    [Space(5)]

    [Header("Weapons")]
    public List<GameObject> weaponList = new List<GameObject>();
    [Space(5)]

    [Header("Sounds")]
    public List<AudioClip> jumpSounds = new List<AudioClip>();
    public List<AudioClip> landSounds = new List<AudioClip>();
    public List<AudioClip> stepSounds = new List<AudioClip>();
    public AudioSource audioSource;
    private float lastFootstepTime;
    public float timeBetweenFootsteps = .2f; 

    [Header("References")]
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public UIScript ui;
    private CameraScript cameraScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ui = GetComponentInChildren<UIScript>();
        cameraScript = GetComponentInChildren<CameraScript>();
        orientation = GetComponent<Transform>();
        rb.freezeRotation = true;
        startPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        inputs();
        groundedChecks();
        changeWeapons();
        wallRun();

        // footstep sounds
        if(rb.velocity.magnitude > 0.2f && lastFootstepTime > timeBetweenFootsteps && jumps == maxJumps)
        {
            audioSource.PlayOneShot(stepSounds[Random.Range(0, landSounds.Count - 1)], 1f);
            lastFootstepTime = 0f;
        }

        else if(grounded)
        {
            // Add with the current speed of the player divided by normal max speed. incase you're zoomin'
            lastFootstepTime += Time.deltaTime * rb.velocity.magnitude/speed;
        }

        // death check
        if(health <= 0)
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }

    void FixedUpdate()
    {
        playerMovement();
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

    void inputs()
    {
        // Input Intake
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        moveJump = Input.GetButtonDown("Jump");
        moveJumpHold = Input.GetAxisRaw("Jump");

        // Jumping stuff!
        if(moveJump)
        {
            playerJump();
        }
    }

    void groundedChecks()
    {
        // Grounded Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.15f, groundLayer);

        // Reset jumps and whatnot.
        if(grounded && jumpCD >= jumpCDMax && rb.drag != groundDrag)
        {
            rb.drag = groundDrag;

            // Audio for landing
            if(!audioSource.isPlaying && jumps < maxJumps)
                audioSource.PlayOneShot(landSounds[Random.Range(0, landSounds.Count - 1)], 1f);

            jumps = maxJumps;

            // Reset jump count visuals   
            ui.updateUI();
        }

        else
            rb.drag = 1;

        // Stop grounded from being recalled while you jump initally.
        if(jumpCD <= jumpCDMax)
        {
            jumpCD += Time.deltaTime;
        }
    }

    void playerMovement()
    {
        // Basic Movement
        moveDir = orientation.forward * moveY + orientation.right * moveX;

        // Slope stuff
        if (slopeHandler())
        {
            rb.AddForce(getSlopeMoveDir() * speed * rb.drag, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * speed * rb.drag, ForceMode.Force);
            }
        }

        // No gravity on slopes to prevent sliding.
        rb.useGravity = !slopeHandler();

        // Apply the speed! Yay!
        rb.AddForce(moveDir.normalized * speed * rb.drag, ForceMode.Force);
    }

    void playerJump()
    {
        // Jumping
        if(jumps > 0 && moveJump)
        {
            jumps -= 1;
            jumpCD = 0;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); 

            // Play jump sounds
            audioSource.PlayOneShot(jumpSounds[Random.Range(0, landSounds.Count - 1)], 1f);

            // Update jump counter visuals
            ui.updateUI();
        }
    }

    private bool slopeHandler()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 getSlopeMoveDir()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
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
    }

    void newWeapon(int alpha)
    {
        foreach(GameObject weapon in weaponList)
        {
            weapon.SetActive(false);
        }

        weaponList[alpha-1].SetActive(true);
    }

    void checkWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, wallDistance);
    }

    void wallRun()
    {
        checkWall();
        
        if(canWallrun())
        {
            if(wallLeft)
            {
                Debug.Log("You can wallrun my guy");
            }
        }
    }
}
