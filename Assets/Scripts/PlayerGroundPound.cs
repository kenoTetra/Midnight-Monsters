using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundPound : MonoBehaviour
{
    [Header("Ground Pound")]
    [SerializeField] private float groundPoundSpeed = 15f;
    bool groundPound;

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
        // Check if trying to slide in air
        if(Input.GetAxisRaw("Crouch") !=  0 && !pm.grounded)
            groundPound = true;
        
        else
            groundPound = false;
    }

    void FixedUpdate()
    {
        if(groundPound)
        {
            pm.rb.AddForce(Vector3.down * groundPoundSpeed, ForceMode.Force);
        }
    }
}
