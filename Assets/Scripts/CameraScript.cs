using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX = 3f;
    public float sensY = 3f;
    private float rotateY;

    public Transform orientation;
    private PlayerScript ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX;   
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;  

        rotateY -= mouseY;
        rotateY = Mathf.Clamp(rotateY, -90f, 90f);
        transform.localEulerAngles = Vector3.right * rotateY;

        orientation.Rotate(Vector3.up * mouseX);
    }
}
