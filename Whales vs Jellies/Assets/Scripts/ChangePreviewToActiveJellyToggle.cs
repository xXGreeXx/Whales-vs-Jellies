using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ChangePreviewToActiveJellyToggle : MonoBehaviour {

	//fixed update
	void FixedUpdate () {

        UnityEngine.UI.Toggle t = gameObject.GetComponent<UnityEngine.UI.ToggleGroup>().ActiveToggles().FirstOrDefault();

        if (t.name.Equals("MoonJellyToggle")) { MainGameHandler.type = MainGameHandler.CreatureTypes.MoonJelly; ShopMenuHandler.ChangeBackground(); }
        if (t.name.Equals("CannonballJellyToggle")) { MainGameHandler.type = MainGameHandler.CreatureTypes.CannonballJelly; ShopMenuHandler.ChangeBackground(); }

    }
}
