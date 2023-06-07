using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlayerMovement : MonoBehaviour
{
    // Inputs
    float moveX;
    float moveY;
    [HideInInspector] public Vector3 moveDir;
    [HideInInspector] public Transform orientation;
    [HideInInspector] public float playerHeight = 2f;

    [Header("Movement")]
    public float speed = 10f;
    [SerializeField] private float artificalGravity = -3f;

    // Slope Handling
    [SerializeField] private float maxSlopeAngle;
    RaycastHit slopeHit;

    [Header("Grounded")]
    [SerializeField] private LayerMask groundLayer;
    [HideInInspector] public bool grounded;
    [SerializeField] private float groundDrag = 5f;

    [Header("Sounds")]
    [SerializeField] private List<AudioClip> stepSounds = new List<AudioClip>();
    [HideInInspector] public AudioSource audioSource;

    [SerializeField] private float timeBetweenFootsteps = .2f;
    float lastFootstepTime; 

    // References
    [HideInInspector] public PauseHandler pauseHandler;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public UIScript ui;
    PlayerCharges pc;
    PlayerSliding ps;
    PlayerWallrun pr;
    
    // Start is called before the first frame update
    void Start()
    {
        pauseHandler = GetComponentInChildren<PauseHandler>();
        rb = GetComponent<Rigidbody>();
        ui = GetComponentInChildren<UIScript>();
        pc = GetComponent<PlayerCharges>();
        ps = GetComponent<PlayerSliding>();
        pr = GetComponent<PlayerWallrun>();
        audioSource = GetComponent<AudioSource>();
        orientation = GetComponent<Transform>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!pauseHandler.paused)
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
        }
        
        CheckGrounded();
        FootstepSound();
    }

    void FixedUpdate()
    {
        if(!ps.sliding)
        {
            moveDir = orientation.forward * moveY + orientation.right * moveX;
            rb.AddForce(moveDir.normalized * speed * rb.drag, ForceMode.Force);
        }

        // Slope stuff
        if(SlopeHandler())
        {
            rb.AddForce(GetSlopeMoveDir() * speed * rb.drag, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * speed * rb.drag, ForceMode.Force);
            }
        }

        // No gravity on slopes to prevent sliding down them while standing.
        rb.useGravity = !SlopeHandler();   
    }

    public bool SlopeHandler()
    {  
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, pc.playerHeight * 0.5f + 0.6f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
    
        return false;
    }

    public Vector3 GetSlopeMoveDir()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    void CheckGrounded()
    {
        // Grounded Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.15f, groundLayer);

        if(grounded)
            rb.drag = groundDrag;

        // Increase downwards gravity in air.
        else
        {
            rb.drag = 1;

            if(!pr.isWallRunning)
                rb.AddForce(Vector3.down * Mathf.Abs(artificalGravity), ForceMode.Force);
        }

    }

    void FootstepSound()
    {
        // footstep sounds
        if(rb.velocity.magnitude > 0.2f && lastFootstepTime > timeBetweenFootsteps && grounded)
        {
            audioSource.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Count - 1)], 1f);
            lastFootstepTime = 0f;
        }

        else if(grounded)
        {
            // Add with the current speed of the player divided by normal max speed. incase you're zoomin'
            lastFootstepTime += Time.deltaTime * rb.velocity.magnitude/speed;
        }
    }
}
