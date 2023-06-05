using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayScript : MonoBehaviour
{
    [Header("Sway")]
    public float smooth;
    public float swayMult;

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMult;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMult;

        Quaternion rotateX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotateY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotate = rotateX * rotateY;

        // Rotate object based on sway.
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotate, smooth * Time.deltaTime);
    }
}
