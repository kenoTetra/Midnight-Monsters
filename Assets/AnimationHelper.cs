using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationHelper : MonoBehaviour
{
    [SerializeField] private Animator FadeCanvas;

    public void FadeIn()
    {
        FadeCanvas.SetTrigger("Fade In");
    }

    public void FadeOut()
    {
        FadeCanvas.SetTrigger("Fade Out");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NextScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex+1);
    }
}
