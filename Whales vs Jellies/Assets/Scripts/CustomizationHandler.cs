using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomizationHandler : MonoBehaviour {
    //define global variables
    Vector3 jellyfishHatPoint = new Vector3(53, 193);
    Vector3 jellyfishGunPoint = new Vector3();
    Vector3 jellyfishEyePoint = new Vector3(-8, 134);
    Vector3 jellyfishMouthPoint = new Vector3(-39, 74);
    Vector3 jellyfishVestPoint = new Vector3(1, -28);

    //on mouse down
    void OnMouseDown()
    {
        //add item to slot
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.ToString().Equals(gameObject.name))
        {
            MainGameHandler.selectedItemInInventory.transform.SetParent(gameObject.transform);
            MainGameHandler.selectedItemInInventory.transform.localPosition = new Vector2(-2.58F, 5.63F);
            MainGameHandler.selectedItemInInventory.transform.localScale = new Vector2(84.02002F, 77.76056F);

            ShopMenuHandler.objectsInInventory.Remove(MainGameHandler.selectedItemInInventory);
        }

        MainGameHandler.selectedItemInInventory.transform.Find("Background").GetComponent<SpriteRenderer>().color = Color.white;

        //put item on preview
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.Equals(ItemData.ItemTypes.HatPanel))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);
            obj.transform.localPosition = jellyfishHatPoint;
            obj.transform.localScale = new Vector2(255, 255);
            obj.GetComponent<SpriteRenderer>().sortingOrder = 2;

            MainGameHandler.hatType = MainGameHandler.hatSpritesMap.FirstOrDefault(x => x.Value == obj.GetComponent<SpriteRenderer>().sprite).Key;
            Destroy(obj.transform.Find("Background").gameObject);
        }
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.Equals(ItemData.ItemTypes.GunPanel))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);
            obj.transform.localPosition = jellyfishGunPoint;
            obj.transform.localScale = new Vector2(255, 255);
            obj.GetComponent<SpriteRenderer>().sortingOrder = 2;

            MainGameHandler.weaponType = MainGameHandler.weaponSpritesMap.FirstOrDefault(x => x.Value == obj.GetComponent<SpriteRenderer>().sprite).Key;
            Destroy(obj.transform.Find("Background").gameObject);
        }
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.Equals(ItemData.ItemTypes.EyepiecePanel))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);
            obj.transform.localPosition = jellyfishEyePoint;
            obj.transform.localScale = new Vector2(255, 255);
            obj.GetComponent<SpriteRenderer>().sortingOrder = 1;

            MainGameHandler.eyepieceType = MainGameHandler.eyepieceSpritesMap.FirstOrDefault(x => x.Value == obj.GetComponent<SpriteRenderer>().sprite).Key;
            Destroy(obj.transform.Find("Background").gameObject);
        }
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.Equals(ItemData.ItemTypes.MouthpiecePanel))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);
            obj.transform.localPosition = jellyfishMouthPoint;
            obj.transform.localScale = new Vector2(255, 255);
            obj.GetComponent<SpriteRenderer>().sortingOrder = 2;

            MainGameHandler.mouthpieceType = MainGameHandler.mouthpieceSpritesMap.FirstOrDefault(x => x.Value == obj.GetComponent<SpriteRenderer>().sprite).Key;
            Destroy(obj.transform.Find("Background").gameObject);
        }
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.Equals(ItemData.ItemTypes.VestPanel))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);
            obj.transform.localPosition = jellyfishVestPoint;
            obj.transform.localScale = new Vector2(725, 435);
            obj.GetComponent<SpriteRenderer>().sortingOrder = 1;

            MainGameHandler.vestType = MainGameHandler.vestSpritesMap.FirstOrDefault(x => x.Value == obj.GetComponent<SpriteRenderer>().sprite).Key;
            Destroy(obj.transform.Find("Background").gameObject);
        }
    }
}
