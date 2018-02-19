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
            bool pass = true;
            foreach (GameObject g in ShopMenuHandler.objectsInInventory)
            {
                if (g.GetComponent<ItemData>().Equals(MainGameHandler.selectedItemInInventory.GetComponent<ItemData>()))
                {
                    pass = false;
                    break;
                }
            }

            if (pass)
            {
                //add item back to inventory
                ItemData data = MainGameHandler.selectedItemInInventory.GetComponent<ItemData>();
                ShopMenuHandler.AddItemToInventory(data.itemType, data.itemSprite);

                //remove item from panel
                for (int index = 0; index < ShopMenuHandler.previewPanel.transform.Find("HatPanel").transform.childCount; index++)
                {
                    GameObject child = ShopMenuHandler.previewPanel.transform.Find("HatPanel").transform.GetChild(index).gameObject;
                    if (child.GetComponent<SpriteRenderer>())
                    {
                        if (child.GetComponent<SpriteRenderer>().sprite.Equals(MainGameHandler.selectedItemInInventory.GetComponent<SpriteRenderer>().sprite))
                        {
                            Destroy(child, 0);
                            break;
                        }
                    }
                }

                //remove item from preview
                for (int index = 0; index < ShopMenuHandler.previewPanel.transform.childCount; index++)
                {
                    GameObject child = ShopMenuHandler.previewPanel.transform.GetChild(index).gameObject;
                    if (child.GetComponent<SpriteRenderer>())
                    {
                        if (child.GetComponent<SpriteRenderer>().sprite.Equals(MainGameHandler.selectedItemInInventory.GetComponent<SpriteRenderer>().sprite))
                        {
                            Destroy(child, 0);
                            break;
                        }
                    }
                }

                //delete object
                Destroy(MainGameHandler.selectedItemInInventory, 0);
                MainGameHandler.selectedItemInInventory = null;
            }
        }
    }
}
