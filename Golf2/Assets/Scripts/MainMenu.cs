using System;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{


    public void Start_Button()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);

    }
    public void Player_Button()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);

    }
   
    public void Quit_Button()
    {
        
        Application.Quit();
    }
}
