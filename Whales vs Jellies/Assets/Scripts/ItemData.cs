using UnityEngine;

public class ItemData : MonoBehaviour {
    //enums
    public enum ItemTypes
    {
        HatPanel,
        GunPanel,
        EyepiecePanel,
        MouthpiecePanel,
        VestPanel
    }

    //define global variables
    public ItemTypes itemType;
    public Sprite itemSprite;
    public string description = "";

    //mouse down on item
    void OnMouseDown()
    {
        if (MainGameHandler.selectedItemInInventory != null)
        {
            if (MainGameHandler.selectedItemInInventory.transform.Find("Background") != null) MainGameHandler.selectedItemInInventory.transform.Find("Background").GetComponent<SpriteRenderer>().color = Color.white;
        }
        MainGameHandler.selectedItemInInventory = gameObject;
        if(gameObject.transform.Find("Background") != null) gameObject.transform.Find("Background").GetComponent<SpriteRenderer>().color = Color.gray;
    }

    //display description on enter
    void OnMouseEnter()
    {
        ShopMenuHandler.descriptionPanel.SetActive(true);

        RectTransform rt = ShopMenuHandler.descriptionPanel.GetComponent<RectTransform>();
        ShopMenuHandler.descriptionPanel.transform.position = gameObject.transform.position + new Vector3(gameObject.GetComponent<SpriteRenderer>().size.x * 4, gameObject.GetComponent<SpriteRenderer>().size.y * 2, 0);
        ShopMenuHandler.descriptionPanel.transform.position = new Vector3(ShopMenuHandler.descriptionPanel.transform.position.x, ShopMenuHandler.descriptionPanel.transform.position.y, -100);
        ShopMenuHandler.descriptionPanel.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = description;
    }

    //remove description on exit
    void OnMouseExit()
    {
        ShopMenuHandler.descriptionPanel.SetActive(false);
    }
}
