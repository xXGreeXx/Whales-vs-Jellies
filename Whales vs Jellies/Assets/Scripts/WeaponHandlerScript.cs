using UnityEngine;

public class WeaponHandlerScript : MonoBehaviour {
    //define global variables
    public int ammo;
    public int maxAmmo;

    //fire weapon
    public void FireWeapon(int damage, float speed, bool localIsWhale, bool sentByRemote)
    {
        if (ammo > 0)
        {
            MainGameHandler.bulletsFiredByPlayer.Add(MainGameHandler.CreateBullet(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.rotation, damage, speed, sentByRemote, localIsWhale));
            ammo--;
        }
    }

    //reload weapon
    public void ReloadWeapon()
    {
        ammo = maxAmmo;
    }
}
