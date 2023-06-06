using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    [Header("Input")]
    private float moveX;
    private float moveY;
    private float moveSlide;
    private Vector3 moveDir;
    private bool moveJump;
    private bool moveDash;
    private float moveJumpHold;
    private Transform orientation;

    [Header("Basic Movement")]
    public float speed = 10f;
    [Space(5)]
    
    [Header("Jumping/Grounded")]
    public float groundDrag;
    public float playerHeight = 2f;
    private bool grounded;
    private float jumpCD = .2f;
    public float jumpCDMax = .2f;
    public LayerMask groundLayer;
    public float jumpForce = 5f;
    [Space(5)]

    [Header("Dashing")]
    public float dashSpeed = 10f;
    [Space(5)]

    [Header("Ground Pound")]
    public float groundPoundSpeed = 15f;
    [Space(5)]
    private bool groundPound;
    private bool pressedSlideAir;

    [Header("Charges")]
    public int charges = 3;
    public int maxCharges = 3;
    [Space(5)]

    [Header("Wallrunning")]
    public float wallDistance = .6f;
    public float minimumJumpHeight = 1.5f;
    [HideInInspector] public bool wallLeft;
    [HideInInspector] public bool wallRight;
    private bool canWallrun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }
    public float wallRunGravity = 1f;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    public float wallJumpForce = 6f;
    public bool isWallRunning;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    public bool onSlope;
    [Space(5)]
    private RaycastHit slopeHit;

    [Header("Sliding")]
    public float slideForce = 40f;
    public float slideBurstSpeed = 15f;
    //public float slideYScale = .5f;
    //private float startYScale;
    private bool sliding;
    private float slideTime = 5f;
    public float timeUntilSlideDecay = 5f;
    public bool hasStoppedSliding;
    [Space(5)]
    private bool slideGivenBurst;

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
    [Space(5)]

    [Header("References")]
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public UIScript ui;
    private CameraScript cameraScript;
    private PauseHandler pauseHandler;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ui = GetComponentInChildren<UIScript>();
        cameraScript = GetComponentInChildren<CameraScript>();
        orientation = GetComponent<Transform>();
        pauseHandler = GetComponent<PauseHandler>();
        rb.freezeRotation = true;
        startPoint = transform.position;

        //startYScale = playerObj.transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(!pauseHandler.paused)
        {
            inputs();
            groundedChecks();
            changeWeapons();
            wallRun();

            // Jumping stuff!
            if(moveJump)
                playerJump();

            // Dash
            if(moveDash)
                playerDash();

            // Slide
            if(moveSlide != 0 && grounded && !hasStoppedSliding)
                sliding = true;

            else
                stopSlide();

            // Check if trying to slide in air
            if(Input.GetButtonDown("Crouch") && !grounded)
                pressedSlideAir = true;

            // Ground pound
            if(moveSlide != 0 && !grounded && pressedSlideAir)
                groundPound = true;

            else
            {
                groundPound = false;
                pressedSlideAir = false;
            }

            // footstep sounds
            if(rb.velocity.magnitude > 0.2f && lastFootstepTime > timeBetweenFootsteps && charges == maxCharges)
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
    }

    void FixedUpdate()
    {
        playerMovement();

        if(sliding)
            startSlide();            
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
        moveSlide = Input.GetAxisRaw("Crouch");
        moveJump = Input.GetButtonDown("Jump");
        moveDash = Input.GetButtonDown("Dash");
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
            if(!audioSource.isPlaying && charges < maxCharges)
                audioSource.PlayOneShot(landSounds[Random.Range(0, landSounds.Count - 1)], 1f);

            charges = maxCharges;

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
        if(!sliding)
            moveDir = orientation.forward * moveY + orientation.right * moveX;

        // Slope stuff
        if(slopeHandler())
        {
            rb.AddForce(getSlopeMoveDir() * speed * rb.drag, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * speed * rb.drag, ForceMode.Force);
            }
        }

        // Ground pound for in air
        if(groundPound)
        {
            rb.AddForce(Vector3.down * groundPoundSpeed, ForceMode.Force);
        }

        // No gravity on slopes to prevent sliding.
        rb.useGravity = !slopeHandler();

        // Lock direction if sliding
        if(!sliding)
            rb.AddForce(moveDir.normalized * speed * rb.drag, ForceMode.Force);
    }

    void playerJump()
    {
        // Jumping
        if(charges > 0 && moveJump && !isWallRunning)
        {
            charges -= 1;
            jumpCD = 0;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); 

            // Play jump sounds
            audioSource.PlayOneShot(jumpSounds[Random.Range(0, landSounds.Count - 1)], 1f);

            // Update jump counter visuals
            ui.updateUI();
        }
    }

    void playerDash()
    {
        if(charges > 0 && moveDash && !isWallRunning)
        {
            charges -= 1;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if(moveDir != Vector3.zero)
                rb.AddForce(moveDir * dashSpeed, ForceMode.Impulse);

            else
                rb.AddForce(Vector3.forward * dashSpeed, ForceMode.Impulse);

            // Play jump sounds
            audioSource.PlayOneShot(jumpSounds[Random.Range(0, landSounds.Count - 1)], 1f);

            // Update jump counter visuals
            ui.updateUI();
        }
    }

    private void startSlide()
    {     
        // Scale down player, stop them from being in the air
        //playerObj.transform.localScale = new Vector3(playerObj.transform.localScale.x, slideYScale, playerObj.transform.localScale.z);
        //rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        // Keep player moving forward while sliding, decay over time.
        if(!slopeHandler() || rb.velocity.y > -0.1f)
        {
            if(!slideGivenBurst && rb.velocity.magnitude < 15)
            {
                rb.AddForce(moveDir.normalized * slideBurstSpeed, ForceMode.Impulse);
                slideGivenBurst = true;
            }

            else
            {
                slideGivenBurst = true;
            }

            rb.AddForce(moveDir.normalized * (slideForce * slideTime/timeUntilSlideDecay), ForceMode.Force);

            if(slideTime > 0.2f)
                slideTime -= Time.deltaTime;

            else if(slideTime > 0f)
            {
                hasStoppedSliding = true;
                slideTime -= Time.deltaTime;
            }

            else
                sliding = false;
        }

        // Sliding down slope.
        else
        {
            rb.AddForce(getSlopeMoveDir() * slideForce, ForceMode.Force);
        }
    }

    private void stopSlide()
    {
        sliding = false;
        slideTime = timeUntilSlideDecay;

        if(moveSlide == 0)
        {
            hasStoppedSliding = false;
            slideGivenBurst = false;
        }
        // Return player to normal.
        //playerObj.transform.localScale = new Vector3(playerObj.transform.localScale.x, startYScale, playerObj.transform.localScale.z);
    }

    private bool slopeHandler()
    {
        if(!grounded)
        {
            return false;
        }
    
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.6f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            onSlope = angle < maxSlopeAngle && angle != 0;
            return angle < maxSlopeAngle && angle != 0;
        }

        else
            onSlope = false;
    
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
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance);
    }

    void wallRun()
    {
        checkWall();

        if(canWallrun())
        {
            if(wallLeft && !grounded)
            {
                startWallRun();
                //Debug.Log("Wallrun initiated: Left");
            }

            else if(wallRight && !grounded)
            {
                startWallRun();
                //Debug.Log("Wallrun initated: Right");
            }

            else
                stopWallRun();
        }

        else
            stopWallRun();
    }

    void startWallRun()
    {
        rb.useGravity = false;

        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        if(Input.GetButtonDown("Jump"))
        {
            if (wallLeft)
            {
                Vector3 wallRunJumpDir = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDir * wallJumpForce * 75, ForceMode.Force);
            }

            else if(wallRight)
            {
                Vector3 wallRunJumpDir = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDir * wallJumpForce * 75, ForceMode.Force);
            }
        }

        isWallRunning = true;
    }

    void stopWallRun()
    {
        rb.useGravity = true;
        isWallRunning = false;
    }
}
