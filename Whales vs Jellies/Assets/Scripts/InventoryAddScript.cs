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
            //add item back to inventory
            ItemData data = MainGameHandler.selectedItemInInventory.GetComponent<ItemData>();
            ShopMenuHandler.AddItemToInventory(data.itemType, data.itemSprite);

            //remove item from preview
            for (int index = 0; index < ShopMenuHandler.previewPanel.transform.Find("vestPanel").transform.childCount; index++)
            {
                GameObject child = ShopMenuHandler.previewPanel.transform.Find("vestPanel").transform.GetChild(index).gameObject;
                if (child.GetComponent<SpriteRenderer>().sprite.Equals(MainGameHandler.selectedItemInInventory.GetComponent<SpriteRenderer>().sprite))
                {
                    Destroy(child, 0);
                    break;
                }
            }

            //delete object
            Destroy(MainGameHandler.selectedItemInInventory, 0);
            MainGameHandler.selectedItemInInventory = null;
        }
    }
}
