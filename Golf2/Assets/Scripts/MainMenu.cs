using System;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static int PlayerCount;

    public void One_Player_Button()
    {
        PlayerCount = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void Two_Player_Button()
    {
        PlayerCount = 2;
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void Three_Player_Button()
    {
        PlayerCount = 3;
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void Four_Player_Button()
    {
        PlayerCount = 4;
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void Start_Button()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);

    }

    public void MainMenu_Button()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);

    }
   
    public void Quit_Button()
    {
        
        Application.Quit();
    }
}
