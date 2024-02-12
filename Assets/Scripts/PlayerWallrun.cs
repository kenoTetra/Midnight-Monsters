using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallrun : MonoBehaviour
{
    [Header("Wallrunning")]
    [SerializeField] private LayerMask WallrunMask;
    [SerializeField] private float wallDistance = .8f;
    [SerializeField] private float minimumJumpHeight = 1f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallRunGravity = 1f;
    [SerializeField] private float wallrunAngleMod = .85f;
    float wallDragTime;
    [SerializeField] private float wallDragMax = .35f;
    //[SerializeField] private float shootOffMod = 2f;
    [HideInInspector] public bool isWallRunning;

    // Raycast Info
    [HideInInspector] public bool wallLeft,wallBackLeft,wallFrontLeft;
    [HideInInspector] public bool wallRight,wallBackRight,wallFrontRight;
    private bool canWallrun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    // References
    BasicPlayerMovement pm;

    // Start is called before the first frame update
    void Start()
    {
       pm = GetComponent<BasicPlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!pm.pauseHandler.paused)
        {
            checkWall();

            if(canWallrun())
            {
                if(wallLeft || wallBackLeft || wallFrontLeft || wallRight || wallBackRight || wallFrontRight)
                    startWallRun();

                else
                    stopWallRun();
            }

            else
                stopWallRun();
        }
    }

    void checkWall()
    {
        // Left
        wallLeft = Physics.Raycast(transform.position, -pm.orientation.right, wallDistance, WallrunMask);
        wallBackLeft = Physics.Raycast(transform.position, (-pm.orientation.right - (pm.orientation.forward/wallrunAngleMod)), wallDistance, WallrunMask);
        wallFrontLeft = Physics.Raycast(transform.position, (-pm.orientation.right + (pm.orientation.forward/wallrunAngleMod)), wallDistance, WallrunMask);

        // Debug Left
        Debug.DrawRay(transform.position, -pm.orientation.right, Color.green);
        Debug.DrawRay(transform.position, -pm.orientation.right - (pm.orientation.forward/wallrunAngleMod), Color.green);
        Debug.DrawRay(transform.position, -pm.orientation.right + (pm.orientation.forward/wallrunAngleMod), Color.green);

        // Right
        wallRight = Physics.Raycast(transform.position, pm.orientation.right, wallDistance, WallrunMask);
        wallBackRight = Physics.Raycast(transform.position, pm.orientation.right - (pm.orientation.forward/wallrunAngleMod), wallDistance, WallrunMask);
        wallFrontRight = Physics.Raycast(transform.position, pm.orientation.right + (pm.orientation.forward/wallrunAngleMod), wallDistance, WallrunMask);

        // Debug Right
        Debug.DrawRay(transform.position, pm.orientation.right, Color.green);
        Debug.DrawRay(transform.position, pm.orientation.right - (pm.orientation.forward/wallrunAngleMod), Color.green);
        Debug.DrawRay(transform.position, pm.orientation.right + (pm.orientation.forward/wallrunAngleMod), Color.green);
    }

    void startWallRun()
    {
        // Continue walldrag
        if(wallDragTime < wallDragMax)
        {
            pm.rb.useGravity = false;
            isWallRunning = true;
            pm.rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
        }
        
        //
        if (wallLeft || wallBackLeft || wallFrontLeft || wallRight || wallBackRight || wallFrontRight)
            wallDragTime = 0f;

        else if(wallDragTime < wallDragMax)
        {
            wallDragTime += Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && wallDragTime < wallDragMax)
        {
            Vector3 wallRunJumpDir = transform.up + pm.orientation.forward;
            pm.rb.velocity = new Vector3(pm.rb.velocity.x, 0, pm.rb.velocity.z);
            pm.rb.AddForce(wallRunJumpDir * wallJumpForce, ForceMode.Impulse);
            wallDragTime = wallDragMax;
        }        
    }

    void stopWallRun()
    {
        pm.rb.useGravity = true;
        isWallRunning = false;
    }
}
