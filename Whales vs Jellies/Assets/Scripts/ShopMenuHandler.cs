using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenuHandler : MonoBehaviour {
    //define global variables
    public static GameObject previewPanel;
    public static GameObject inventoryPanel;
    public static List<GameObject> objectsInInventory = new List<GameObject>();
    public static Vector2 lastPosition;

    //start
    void Start ()
    {
        previewPanel = GameObject.Find("PreviewPanel");
        inventoryPanel = GameObject.Find("InventoryPanel");

        ChangeBackground();

        lastPosition = new Vector2(-158, 222);

        AddItemToInventory(ItemData.ItemTypes.GunPanel, SpriteHandler.jellyfishSpineShooter);
        AddItemToInventory(ItemData.ItemTypes.HatPanel, SpriteHandler.topHatSprite);
        AddItemToInventory(ItemData.ItemTypes.MouthpiecePanel, SpriteHandler.cigarSprite);
        AddItemToInventory(ItemData.ItemTypes.EyepiecePanel, SpriteHandler.sunglassesSprite);
        AddItemToInventory(ItemData.ItemTypes.VestPanel, SpriteHandler.bulletProofVestSprite);
        AddItemToInventory(ItemData.ItemTypes.HatPanel, SpriteHandler.pirateHatSprite);
        AddItemToInventory(ItemData.ItemTypes.EyepiecePanel, SpriteHandler.visorSprite);
    }

    //fixed update
    void FixedUpdate()
    {
        GameObject.Find("XPBarText").GetComponent<UnityEngine.UI.Text>().text = MainGameHandler.playerCurrentXP + "/" + MainGameHandler.xpNeededForNextLevel;
        GameObject.Find("goldText").GetComponent<UnityEngine.UI.Text>().text = MainGameHandler.currency.ToString();
    }

    //change preview base
    public static void ChangeBackground()
    {
        if (MainGameHandler.isWhale)
        {
            previewPanel.GetComponent<UnityEngine.UI.Image>().sprite = SpriteHandler.whaleSprite;
            previewPanel.transform.rotation = Quaternion.Euler(0, 0, 270);
            previewPanel.transform.localScale = new Vector2(-1, 1);
        }
        else
        {
            previewPanel.GetComponent<UnityEngine.UI.Image>().sprite = SpriteHandler.jellyFishSprite;
            previewPanel.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    //add item to inventory
    public static void AddItemToInventory(ItemData.ItemTypes type, Sprite sprite)
    {
        GameObject item = new GameObject("Item");
        item.transform.SetParent(inventoryPanel.transform);
        item.transform.localPosition = lastPosition;
        item.AddComponent<BoxCollider2D>();
        ItemData data = item.AddComponent<ItemData>();
        data.itemType = type;
        data.itemSprite = sprite;

        SpriteRenderer renderer = item.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = 2;

        float shift = 244.5F / 3.25F;

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
    }
}
