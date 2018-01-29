using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationHandler : MonoBehaviour {
    //define global variables

    //on mouse down
    void OnMouseDown()
    {
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.ToString().Equals(gameObject.name))
        {
            MainGameHandler.selectedItemInInventory.transform.SetParent(gameObject.transform);
            MainGameHandler.selectedItemInInventory.transform.localPosition = new Vector2(-2.58F, 5.63F);
            MainGameHandler.selectedItemInInventory.transform.localScale = new Vector2(84.02002F, 77.76056F);

            ShopMenuHandler.objectsInInventory.Remove(MainGameHandler.selectedItemInInventory);
        }

        MainGameHandler.selectedItemInInventory.transform.Find("Background").GetComponent<SpriteRenderer>().color = Color.white;
    }
}
