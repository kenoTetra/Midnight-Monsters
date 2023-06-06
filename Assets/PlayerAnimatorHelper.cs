using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorHelper : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    public void StopFireAnim()
    {
        animator.SetBool("Fire", false);
    }
}
