using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    private PauseHandler pauseHandler;

    // Start is called before the first frame update
    void Start()
    {
        pauseHandler = GameObject.FindWithTag("Player").GetComponent<PauseHandler>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            resumeGame();
        }
    }

    public void resumeGame()
    {
        Time.timeScale = 1.0f;
        pauseHandler.paused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        Destroy(this.gameObject);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
