﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScript : MonoBehaviour {

    public void OnButtonPressed(string button)
    {
        if (button.Equals("Play"))
        {
            SceneManager.LoadScene("Game");
        }
        else if (button.Equals("Shop"))
        {
            SceneManager.LoadScene("Shop");
        }
        else if (button.Equals("Quit"))
        {
            Application.Quit();
        }
    }
}
