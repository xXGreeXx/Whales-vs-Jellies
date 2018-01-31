using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAddScript : MonoBehaviour {

	//start
	void Start ()
    {
		
	}

    //on mouse click
    void OnMouseDown()
    {
        if (MainGameHandler.selectedItemInInventory != null)
        {
            ItemData data = MainGameHandler.selectedItemInInventory.GetComponent<ItemData>();
            ShopMenuHandler.AddItemToInventory(data.itemType, data.itemSprite);

            Destroy(MainGameHandler.selectedItemInInventory, 0);
        }
    }
}
