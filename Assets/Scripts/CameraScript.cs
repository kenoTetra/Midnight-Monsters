using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensX = 3f;
    public float sensY = 3f;
    [Space(5)]
    private float rotateY;

    [Header("FoV Changes")]
    public float fov = 90f;
    private float wallRunfov;
    public float camTilt;
    public float wallEffectTime = 10f;
    public float tilt { get; set; }

    public Transform orientation;
    private PlayerScript ps;
    private Camera cam;
    private PauseHandler pauseHandler;

    // Start is called before the first frame update
    void Start()
    {
        ps = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
        pauseHandler = GameObject.FindWithTag("Player").GetComponent<PauseHandler>();
        cam = GetComponent<Camera>();
        wallRunfov = fov + 10f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!pauseHandler.paused)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * sensX;   
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;  

            rotateY -= mouseY;
            rotateY = Mathf.Clamp(rotateY, -90f, 90f);
            transform.localEulerAngles = Vector3.right * rotateY + new Vector3(0f, 0f, tilt);

            orientation.Rotate(Vector3.up * mouseX);

            // Funny camera effects when wallrunning.
            
            if(ps.isWallRunning)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunfov, wallEffectTime * Time.deltaTime);

                if(ps.wallLeft)
                    tilt = Mathf.Lerp(tilt, -camTilt, wallEffectTime * Time.deltaTime);

                else if(ps.wallRight)
                    tilt = Mathf.Lerp(tilt, camTilt, wallEffectTime * Time.deltaTime);
            }

            else if(cam.fieldOfView != fov || tilt != 0f)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallEffectTime * Time.deltaTime);
                tilt = Mathf.Lerp(tilt, 0, wallEffectTime * Time.deltaTime);
            }
        }
    }
}
