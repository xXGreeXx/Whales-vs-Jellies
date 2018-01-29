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
        if (MainGameHandler.selectedItemInInventory != null && MainGameHandler.selectedItemInInventory.Equals(gameObject))
        {
            MainGameHandler.selectedItemInInventory = null;
            gameObject.transform.Find("Background").GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            MainGameHandler.selectedItemInInventory = gameObject;
            gameObject.transform.Find("Background").GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }
}
