using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSliding : MonoBehaviour
{
    [Header("Sliding")]
    [SerializeField] private float slideForce = 40f;
    [SerializeField] private float slideBurstSpeed = 15f;
    bool slideGivenBurst;

    //public float slideYScale = .5f;
    //private float startYScale;

    [HideInInspector] public bool sliding;

    public float timeUntilSlideDecay = 5f;
    float slideTime;

    // References
    BasicPlayerMovement pm;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<BasicPlayerMovement>();
        slideTime = timeUntilSlideDecay;
    }

    // Update is called once per frame
    void Update()
    {
        // Slide
        if(Input.GetAxisRaw("Crouch") != 0 && pm.grounded)
            sliding = true;

        else
            StopSlide();
    }

    void FixedUpdate()
    {
        if(sliding)
            StartSlide();
    }

    private void StartSlide()
    {     
        // Scale down player, stop them from being in the air
        //playerObj.transform.localScale = new Vector3(playerObj.transform.localScale.x, slideYScale, playerObj.transform.localScale.z);
        //rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        // Keep player moving forward while sliding, decay over time.
        if(!pm.SlopeHandler() || pm.rb.velocity.y > -0.1f)
        {
            if(!slideGivenBurst && pm.rb.velocity.magnitude < 15)
            {
                pm.rb.AddForce(pm.moveDir.normalized * slideBurstSpeed, ForceMode.Impulse);
                slideGivenBurst = true;
            }

            else if(pm.rb.velocity.magnitude >= 15)
                slideGivenBurst = true;

            pm.rb.AddForce(pm.moveDir.normalized * slideForce, ForceMode.Force);;
        }

        // Sliding down slope velocity.
        else
        {
            pm.rb.AddForce(pm.GetSlopeMoveDir() * slideForce, ForceMode.Force);
        }
    }

    public void StopSlide()
    {
        sliding = false;
        slideTime = timeUntilSlideDecay;
        slideGivenBurst = false;
        
        // Return player to normal.
        //playerObj.transform.localScale = new Vector3(playerObj.transform.localScale.x, startYScale, playerObj.transform.localScale.z);
    }
}
