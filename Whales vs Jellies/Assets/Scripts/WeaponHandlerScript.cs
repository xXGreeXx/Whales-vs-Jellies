using UnityEngine;

public class WeaponHandlerScript : MonoBehaviour {
    //define global variables
    public int ammo;
    public int maxAmmo;

    //fire weapon
    public void FireWeapon(int damage)
    {
        if (ammo > 0)
        {
            MainGameHandler.bulletsFiredByPlayer.Add(MainGameHandler.CreateBullet(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.rotation, damage, 10));
            ammo--;
        }
    }

    //reload weapon
    public void ReloadWeapon()
    {
        ammo = maxAmmo;
    }
}
