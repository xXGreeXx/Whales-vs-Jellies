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
            MainGameHandler.Disconnect();
            MainGameHandler.ExitAndSave();
        }

        //shop
        if (button.Equals("Back"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if (button.Equals("GoWhale"))
        {
            MainGameHandler.isWhale = true;
            MainGameHandler.type = MainGameHandler.CreatureTypes.BottleNose;
            ShopMenuHandler.ChangeBackground();
        }

        //game
        if (button.Equals("Return"))
        {
            MainGameHandler.escapeMenuPanel.SetActive(false);
        }
        else if (button.Equals("DC"))
        {
            MainGameHandler.Disconnect();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
