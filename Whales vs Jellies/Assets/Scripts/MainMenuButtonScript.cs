using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScript : MonoBehaviour {

    public void OnButtonPressed(string button)
    {
        //main menu
        if (button.Equals("Play"))
        {
            MainGameHandler.IP = GameObject.Find("IPField").transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text;
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

        //shop
        if (button.Equals("Back"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if (button.Equals("GoWhale"))
        {
            MainGameHandler.isWhale = true;
        }
    }
}
