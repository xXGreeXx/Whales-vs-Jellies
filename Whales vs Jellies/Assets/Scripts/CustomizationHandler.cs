using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomizationHandler : MonoBehaviour {
    //define global variables

    #region MoonJelly
    Vector3 moonjellyfishHatPoint = new Vector3(10.8F, 172.6F);
    Vector3 moonjellyfishHatScale = new Vector3(255, 255);

    Vector3 moonjellyfishGunPoint = new Vector3(35, -82);
    Vector3 moonjellyfishGunScale = new Vector3(255, 255);

    Vector3 moonjellyfishEyePoint = new Vector3(-13, 97);
    Vector3 moonjellyfishEyeScale = new Vector3(255, 255);

    Vector3 moonjellyfishMouthPoint = new Vector3(-53, 40);
    Vector3 moonjellyfishMouthScale = new Vector3(255, 255);

    Vector3 moonjellyfishVestPoint = new Vector3(24, -91);
    Vector3 moonjellyfishVestScale = new Vector3(255, 255);
    #endregion

    #region BottleNose
    Vector3 bottlenoseHatPoint = new Vector3(72.7F, 119.6F);
    Vector3 bottlenoseHatScale = new Vector3(255, 255);

    Vector3 bottlenoseGunPoint = new Vector3(-29, 34);
    Vector3 bottlenoseGunScale = new Vector3(255, 255);

    Vector3 bottlenoseEyePoint = new Vector3(19, 140.4F);
    Vector3 bottlenoseEyeScale = new Vector3(255, 255);

    Vector3 bottlenoseMouthPoint = new Vector3(-34.9F, 174);
    Vector3 bottlenoseMouthScale = new Vector3(255, 255);

    Vector3 bottlenoseVestPoint = new Vector3(4, -4);
    Vector3 bottlenoseVestScale = new Vector3(255, 255);
    #endregion

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
            ShopMenuHandler.PushItems();
        }

        MainGameHandler.selectedItemInInventory.transform.Find("Background").GetComponent<SpriteRenderer>().color = Color.white;

        //put item on preview
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.ToString().Equals("HatPanel"))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);

            Vector2 attachPoint = Vector2.zero;
            Vector2 scalePoint = Vector2.zero;

            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.MoonJelly)) { attachPoint = moonjellyfishHatPoint; scalePoint = moonjellyfishHatScale; }
            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.BottleNose)){ attachPoint = bottlenoseHatPoint; scalePoint = bottlenoseHatScale; }

            obj.transform.localPosition = attachPoint;
            obj.transform.localScale = scalePoint;

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
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.ToString().Equals("GunPanel"))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);

            Vector2 attachPoint = Vector2.zero;
            Vector2 scalePoint = Vector2.zero;

            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.MoonJelly)) { attachPoint = moonjellyfishGunPoint; scalePoint = moonjellyfishGunScale; }
            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.BottleNose)) { attachPoint = bottlenoseGunPoint;  scalePoint = bottlenoseGunScale; }

            obj.transform.localPosition = attachPoint;
            obj.transform.localScale = scalePoint;

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
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.ToString().Equals("EyepiecePanel"))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);

            Vector2 attachPoint = Vector2.zero;
            Vector2 scalePoint = Vector2.zero;

            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.MoonJelly)) { attachPoint = moonjellyfishEyePoint; scalePoint = moonjellyfishEyeScale; }
            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.BottleNose)) { attachPoint = bottlenoseEyePoint; scalePoint = bottlenoseEyeScale; }

            obj.transform.localPosition = attachPoint;
            obj.transform.localScale = scalePoint;

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
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.ToString().Equals("MouthpiecePanel"))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);

            Vector2 attachPoint = Vector2.zero;
            Vector2 scalePoint = Vector2.zero;

            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.MoonJelly)) { attachPoint = moonjellyfishMouthPoint; scalePoint = moonjellyfishMouthScale; }
            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.BottleNose)) { attachPoint = bottlenoseMouthPoint; scalePoint = bottlenoseMouthScale; }

            obj.transform.localPosition = attachPoint;
            obj.transform.localScale = scalePoint;

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
        if (MainGameHandler.selectedItemInInventory.GetComponent<ItemData>().itemType.ToString().Equals("VestPanel"))
        {
            GameObject obj = GameObject.Instantiate(MainGameHandler.selectedItemInInventory, ShopMenuHandler.previewPanel.transform);

            Vector2 attachPoint = Vector2.zero;
            Vector2 scalePoint = Vector2.zero;

            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.MoonJelly)) { attachPoint = moonjellyfishVestPoint; scalePoint = moonjellyfishVestScale; }
            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.BottleNose)) { attachPoint = bottlenoseVestPoint; scalePoint = bottlenoseVestScale; }
            
            obj.transform.localPosition = attachPoint;
            obj.transform.localScale = scalePoint;

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
