 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void PlayTraining() 
    {
        SceneManager.LoadScene("TrainingScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
