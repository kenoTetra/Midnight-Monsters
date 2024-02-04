using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Animator cam_anim;
    [SerializeField] private Animator allScene;

    public void QuitGame()
    {
        cam_anim.SetTrigger("ExitGame");
        allScene.SetTrigger("ExitGame");
    }

    public void NextScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex+1);
    }

    public void Settings()
    {
        cam_anim.SetTrigger("Settings");
    }

    public void ReturnFromSettings()
    {
        cam_anim.SetTrigger("Main");
    }

    public void ConfirmQuit()
    {
        cam_anim.SetTrigger("Quit");
    }

    public void ReturnFromQuit()
    {
        cam_anim.SetTrigger("MainFromQuit");
    }

    public void Credits()
    {
        cam_anim.SetTrigger("Credits");
    }

    public void SettingsFromCredits()
    {
        cam_anim.SetTrigger("SettingsFromCredits");
    }
}
