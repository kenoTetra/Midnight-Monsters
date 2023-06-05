using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool jumping;
    private float jumpCD = .2f;
    public float jumpCDMax = .2f;
    public LayerMask groundLayer;
    public float jumpForce = 5f;
    public int jumps = 2;
    public int maxJumps = 2;
    [Space(5)]

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    [Space(5)]
    private RaycastHit slopeHit;

    [Header("Checkpoints")]
    public Vector3 startPoint;
    public Vector3 checkpoint;

    [Header("References")]
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public UIScript ui;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ui = GetComponentInChildren<UIScript>();
        orientation = GetComponent<Transform>();
        rb.freezeRotation = true;
        startPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        inputs();
        groundedChecks();
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
            {
                transform.position = checkpoint;
            }

            else
            {
                transform.position = startPoint;
            }
        }
    }

    void inputs()
    {
        // Input Intake
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        moveJump = Input.GetButtonDown("Jump");
        moveJumpHold = Input.GetAxisRaw("Jump");

        // Stop empty jump calls
        if(moveJump && !jumping)
        {
            FixedUpdate();
        }
    }

    void groundedChecks()
    {
        // Grounded Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.15f, groundLayer);

        // Reset jumps and whatnot.
        if(grounded && jumpCD >= jumpCDMax)
        {
            rb.drag = groundDrag;
            jumps = maxJumps;
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

        rb.AddForce(moveDir.normalized * speed * rb.drag, ForceMode.Force);

        // Jumping
        if(jumps > 0 && moveJump)
        {
            jumping = true;
            jumps -= 1;
            jumpCD = 0;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); 
            ui.updateUI();
            jumping = false;
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
}
