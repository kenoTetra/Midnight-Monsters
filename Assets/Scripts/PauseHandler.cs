using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    public GameObject pausePrefab;
    private GameObject spawnedPausePrefab;
    [HideInInspector] public bool paused;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Pause") && !paused)
        {
            spawnedPausePrefab = Instantiate(pausePrefab, transform.position, Quaternion.identity);
            Time.timeScale = 0.0f;
            paused = true;
        }

        else if(Input.GetButtonDown("Pause") && paused)
        {
            Destroy(spawnedPausePrefab);
            Time.timeScale = 1.0f;
            paused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
