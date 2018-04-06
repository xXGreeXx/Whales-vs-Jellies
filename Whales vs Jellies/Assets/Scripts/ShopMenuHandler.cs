using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenuHandler : MonoBehaviour {
    //define global variables
    public static GameObject previewPanel;
    public static GameObject inventoryPanel;
    public static GameObject descriptionPanel;
    public static List<GameObject> objectsInInventory = new List<GameObject>();
    public static Vector3 lastPosition;
    private static float shift = 244.5F / 3.25F;

    public static GameObject shopMenuForJellies;
    public static GameObject shopMenuForWhales;

    //start
    void Start ()
    {
        MainGameHandler.isWhale = false;

        previewPanel = GameObject.Find("PreviewPanel");
        inventoryPanel = GameObject.Find("InventoryPanel");
        descriptionPanel = GameObject.Find("descriptionPanel");
        shopMenuForJellies = GameObject.Find("ShopPanelForJellies");
        shopMenuForWhales = GameObject.Find("ShopPanelForWhales");

        descriptionPanel.SetActive(false);

        ChangeBackground();

        lastPosition = new Vector3(-158, 222, -15);

        AddItemToInventory(ItemData.ItemTypes.GunPanel, SpriteHandler.nematocystSpriteLoaded, "Nematocyst \nThis weapon packs a sting! High single target damage.");
        AddItemToInventory(ItemData.ItemTypes.GunPanel, SpriteHandler.harpoonGunSprite, "HarpoonGun \nHook your foes!");
        AddItemToInventory(ItemData.ItemTypes.GunPanel, SpriteHandler.omeletteBringerSprite, "OmeletteBringer \n <Description here>");
        AddItemToInventory(ItemData.ItemTypes.HatPanel, SpriteHandler.topHatSprite, "TopHat \nWant to impress your friends? Look like a businessman? This is for you!");
        AddItemToInventory(ItemData.ItemTypes.MouthpiecePanel, SpriteHandler.cigarSprite, "Cigar \nThis just raises further questions.");
        AddItemToInventory(ItemData.ItemTypes.EyepiecePanel, SpriteHandler.sunglassesSprite, "Sunglasses \nBlock harmful ultraviolet rays AND looks great!");
        AddItemToInventory(ItemData.ItemTypes.VestPanel, SpriteHandler.bulletProofVestSprite, "BulletproofVest \nIf you die, money back guaranteed!");
        AddItemToInventory(ItemData.ItemTypes.HatPanel, SpriteHandler.pirateHatSprite, "PirateHat \nSteal the treasures of your foes!");
        AddItemToInventory(ItemData.ItemTypes.EyepiecePanel, SpriteHandler.visorSprite, "Visor \nGreat eye protection + solar eclipse viewing or your money back!");

        MainGameHandler.type = MainGameHandler.CreatureTypes.MoonJelly;
    }

    //fixed update
    void FixedUpdate()
    {
        GameObject.Find("XPBarText").GetComponent<UnityEngine.UI.Text>().text = MainGameHandler.playerCurrentXP + "/" + MainGameHandler.xpNeededForNextLevel;
        GameObject.Find("goldText").GetComponent<UnityEngine.UI.Text>().text = MainGameHandler.currency.ToString();
        GameObject.Find("levelText").GetComponent<UnityEngine.UI.Text>().text = "Lvl: " + MainGameHandler.playerLevel.ToString();

        GameObject.Find("attackLabel").GetComponent<UnityEngine.UI.Text>().text = MainGameHandler.attack.ToString();
        GameObject.Find("defenseLabel").GetComponent<UnityEngine.UI.Text>().text = MainGameHandler.defense.ToString();
        GameObject.Find("healthLabel").GetComponent<UnityEngine.UI.Text>().text = MainGameHandler.health.ToString();
        GameObject.Find("speedLabel").GetComponent<UnityEngine.UI.Text>().text = MainGameHandler.speed.ToString();
    }

    //change preview base
    public static void ChangeBackground()
    {
        //clear existing stuff
        ClearPreview();

        //change
        if (MainGameHandler.isWhale)
        {
            shopMenuForJellies.SetActive(false);
            shopMenuForWhales.SetActive(true);

            if(MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.BottleNose)) previewPanel.GetComponent<UnityEngine.UI.Image>().sprite = SpriteHandler.bottleNoseSprite;
            previewPanel.transform.rotation = Quaternion.Euler(0, 0, 270);
            previewPanel.transform.localScale = new Vector2(-1, -1);

            for (int index = 0; index < previewPanel.transform.childCount; index++)
            {
                GameObject child = previewPanel.transform.GetChild(index).gameObject;

                child.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
        }
        else
        {
            shopMenuForJellies.SetActive(true);
            shopMenuForWhales.SetActive(false);

            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.MoonJelly)) previewPanel.GetComponent<UnityEngine.UI.Image>().sprite = SpriteHandler.moonJellySprite;
            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.CannonballJelly)) previewPanel.GetComponent<UnityEngine.UI.Image>().sprite = SpriteHandler.cannonballJellySprite;
            if (MainGameHandler.type.Equals(MainGameHandler.CreatureTypes.BluefireJelly)) previewPanel.GetComponent<UnityEngine.UI.Image>().sprite = SpriteHandler.bluefireJellySprite;
            previewPanel.transform.rotation = Quaternion.Euler(0, 0, 0);

            for (int index = 0; index < previewPanel.transform.childCount; index++)
            {
                GameObject child = previewPanel.transform.GetChild(index).gameObject;

                child.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    //push items to right spot
    public static void PushItems()
    {
        lastPosition = new Vector3(-158, 222, -15);
        for (int index = 0; index < inventoryPanel.transform.childCount; index++)
        {
            GameObject item = inventoryPanel.transform.GetChild(index).gameObject;

            if (item.name.Equals("Item"))
            {
                item.transform.localPosition = lastPosition;

                lastPosition.x += shift;
                if (lastPosition.x + 158 > shift * 5)
                {
                    lastPosition.x = -158;
                    lastPosition.y -= shift;
                }
            }
        }
    }

    //add item to inventory
    public static void AddItemToInventory(ItemData.ItemTypes type, Sprite sprite, String desc)
    {
        GameObject item = new GameObject("Item");
        item.transform.SetParent(inventoryPanel.transform);
        item.transform.localPosition = lastPosition;
        item.AddComponent<BoxCollider2D>();
        ItemData data = item.AddComponent<ItemData>();
        data.itemType = type;
        data.itemSprite = sprite;
        data.description = desc;

        SpriteRenderer renderer = item.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 2;

        lastPosition.x += shift;
        if (lastPosition.x + 158 > shift * 5)
        {
            lastPosition.x = -158;
            lastPosition.y -= shift;
        }

        GameObject background = new GameObject("Background");
        background.transform.SetParent(item.transform);
        background.transform.position = item.transform.position - new Vector3(-0.03F, 0.064F, 0);
        background.transform.localScale = new Vector2(0.75F, 0.75F);

        SpriteRenderer backgroundRenderer = background.AddComponent<SpriteRenderer>();
        backgroundRenderer.sortingOrder = 1;
        backgroundRenderer.sprite = SpriteHandler.backgroundSprite;

        PushItems();
    }

    //remove items from preview
    public static void ClearPreview()
    {
        //remove item from panel
        for (int index = 0; index < previewPanel.transform.childCount; index++)
        {
            GameObject child = previewPanel.transform.GetChild(index).gameObject;
            for (int child2Index = 0; child2Index < child.transform.childCount; child2Index++)
            {
                GameObject child2 = child.transform.GetChild(child2Index).gameObject;
                if (child2.GetComponent<SpriteRenderer>())
                {
                    Destroy(child2, 0);
                }
            }
        }

        //remove item from preview
        for (int index = 0; index < previewPanel.transform.childCount; index++)
        {
            GameObject child = previewPanel.transform.GetChild(index).gameObject;
            if (child.GetComponent<SpriteRenderer>())
            {
                Destroy(child, 0);
            }
        }
    }
}
