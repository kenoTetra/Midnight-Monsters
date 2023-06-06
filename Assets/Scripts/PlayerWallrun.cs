using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallrun : MonoBehaviour
{
    [Header("Wallrunning")]
    [SerializeField] private float wallDistance = .6f;
    [SerializeField] private float minimumJumpHeight = 1.5f;
    [SerializeField] private float wallJumpForce = 40f;
    [SerializeField] private float wallRunGravity = 1f;
    [HideInInspector] public bool isWallRunning;

    // Raycast Info
    [HideInInspector] public bool wallLeft;
    [HideInInspector] public bool wallRight;
    RaycastHit leftWallHit;
    RaycastHit rightWallHit;
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
                if(wallLeft && !pm.grounded)
                {
                    startWallRun();
                    //Debug.Log("Wallrun initiated: Left");
                }

                else if(wallRight && !pm.grounded)
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
    }

    void checkWall()
    {
        wallLeft = Physics.Raycast(transform.position, -pm.orientation.right, out leftWallHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, pm.orientation.right, out rightWallHit, wallDistance);
    }

    void startWallRun()
    {
        pm.rb.useGravity = false;

        pm.rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        if(Input.GetButtonDown("Jump"))
        {
            if (wallLeft)
            {
                Vector3 wallRunJumpDir = transform.up + leftWallHit.normal;
                pm.rb.velocity = new Vector3(pm.rb.velocity.x, 0, pm.rb.velocity.z);
                pm.rb.AddForce(wallRunJumpDir * wallJumpForce * 75, ForceMode.Force);
            }

            else if(wallRight)
            {
                Vector3 wallRunJumpDir = transform.up + rightWallHit.normal;
                pm.rb.velocity = new Vector3(pm.rb.velocity.x, 0, pm.rb.velocity.z);
                pm.rb.AddForce(wallRunJumpDir * wallJumpForce * 75, ForceMode.Force);
            }
        }

        isWallRunning = true;
    }

    void stopWallRun()
    {
        pm.rb.useGravity = true;
        isWallRunning = false;
    }
}
