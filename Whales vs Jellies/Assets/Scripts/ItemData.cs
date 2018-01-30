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
}
