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

    Vector3 whaleHatPoint = new Vector3(74, 98);
    Vector3 whaleGunPoint = new Vector3();
    Vector3 whaleEyePoint = new Vector3(-8, 134);
    Vector3 whaleMouthPoint = new Vector3(-50, 150);
    Vector3 whaleVestPoint = new Vector3(8, -25);

    //on mouse down
    void OnMouseDown()
    {
        //add item to slot
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.ToString().Equals(gameObject.name))
        {
            MainGameHandler.selectedItemInInventory.transform.SetParent(gameObject.transform);
            MainGameHandler.selectedItemInInventory.transform.localPosition = new Vector3(-2.58F, 5.63F, -15);
            MainGameHandler.selectedItemInInventory.transform.localScale = new Vector2(84.02002F, 77.76056F);
            MainGameHandler.selectedItemInInventory.transform.localRotation = Quaternion.Euler(0, 0, 0);

            ShopMenuHandler.objectsInInventory.Remove(MainGameHandler.selectedItemInInventory);
        }

        MainGameHandler.selectedItemInInventory.transform.Find("Background").GetComponent<SpriteRenderer>().color = Color.white;

        //put item on preview
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.Equals(ItemData.ItemTypes.HatPanel))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);
            obj.transform.localPosition = MainGameHandler.isWhale ? whaleHatPoint : jellyfishHatPoint;
            obj.transform.localScale = new Vector2(255, 255);
            if (MainGameHandler.isWhale)
            {
                obj.transform.localRotation = Quaternion.Euler(0, 0, 90);
                obj.transform.rotation = Quaternion.Euler(0, 0, 0);
                obj.transform.localScale *= -1;
            }
            obj.GetComponent<SpriteRenderer>().sortingOrder = 2;
            Destroy(obj.GetComponent<BoxCollider2D>());

            MainGameHandler.hatType = MainGameHandler.hatSpritesMap.FirstOrDefault(x => x.Value == obj.GetComponent<SpriteRenderer>().sprite).Key;
            Destroy(obj.transform.Find("Background").gameObject);
        }
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.Equals(ItemData.ItemTypes.GunPanel))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);
            obj.transform.localPosition = MainGameHandler.isWhale ? whaleGunPoint : jellyfishGunPoint;
            obj.transform.localScale = new Vector2(255, 255);
            if (MainGameHandler.isWhale)
            {
                obj.transform.localRotation = Quaternion.Euler(0, 0, 90);
                obj.transform.rotation = Quaternion.Euler(0, 0, 0);
                obj.transform.localScale *= -1;
            }
            obj.GetComponent<SpriteRenderer>().sortingOrder = 2;
            Destroy(obj.GetComponent<BoxCollider2D>());

            MainGameHandler.weaponType = MainGameHandler.weaponSpritesMap.FirstOrDefault(x => x.Value == obj.GetComponent<SpriteRenderer>().sprite).Key;
            Destroy(obj.transform.Find("Background").gameObject);
        }
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.Equals(ItemData.ItemTypes.EyepiecePanel))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);
            obj.transform.localPosition = MainGameHandler.isWhale ? whaleEyePoint : jellyfishEyePoint;
            obj.transform.localScale = new Vector2(255, 255);
            if (MainGameHandler.isWhale)
            {
                obj.transform.localRotation = Quaternion.Euler(0, 0, 90);
                obj.transform.rotation = Quaternion.Euler(0, 0, 0);
                obj.transform.localScale *= -1;
            }
            obj.GetComponent<SpriteRenderer>().sortingOrder = 1;
            Destroy(obj.GetComponent<BoxCollider2D>());

            MainGameHandler.eyepieceType = MainGameHandler.eyepieceSpritesMap.FirstOrDefault(x => x.Value == obj.GetComponent<SpriteRenderer>().sprite).Key;
            Destroy(obj.transform.Find("Background").gameObject);
        }
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.Equals(ItemData.ItemTypes.MouthpiecePanel))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);
            obj.transform.localPosition = MainGameHandler.isWhale ? whaleMouthPoint : jellyfishMouthPoint;
            obj.transform.localScale = new Vector2(255, 255);
            if (MainGameHandler.isWhale)
            {
                obj.transform.localRotation = Quaternion.Euler(0, 0, 90);
                obj.transform.rotation = Quaternion.Euler(0, 0, 0);
                obj.transform.localScale *= -1;
            }
            obj.GetComponent<SpriteRenderer>().sortingOrder = 2;
            Destroy(obj.GetComponent<BoxCollider2D>());

            MainGameHandler.mouthpieceType = MainGameHandler.mouthpieceSpritesMap.FirstOrDefault(x => x.Value == obj.GetComponent<SpriteRenderer>().sprite).Key;
            Destroy(obj.transform.Find("Background").gameObject);
        }
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.Equals(ItemData.ItemTypes.VestPanel))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);
            obj.transform.localPosition = MainGameHandler.isWhale ? whaleVestPoint : jellyfishVestPoint;
            obj.transform.localScale = new Vector2(725, 435);
            if (MainGameHandler.isWhale)
            {
                obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
                obj.transform.rotation = Quaternion.Euler(0, 0, 90);
                obj.transform.localScale = new Vector2(-512, -429);


            }
            obj.GetComponent<SpriteRenderer>().sortingOrder = 1;
            Destroy(obj.GetComponent<BoxCollider2D>());

            MainGameHandler.vestType = MainGameHandler.vestSpritesMap.FirstOrDefault(x => x.Value == obj.GetComponent<SpriteRenderer>().sprite).Key;
            Destroy(obj.transform.Find("Background").gameObject);
        }

        MainGameHandler.selectedItemInInventory = null;
    }
}
