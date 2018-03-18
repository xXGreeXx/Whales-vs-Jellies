using UnityEngine;

public class WeaponHandlerScript : MonoBehaviour {
    //define global variables
    public int ammo;

    public int maxAmmo;
    public int damage;
    public float firingSpeed;
    public float bulletSpeed;
    public bool fullyAuto = false;

    private float round = 0;
    private bool canFire = true;

    //fixed update
    void FixedUpdate()
    {
        round += 0.3F;
        if (round >= firingSpeed)
        {
            round = 0;
            canFire = true;
        }
    }

    //fire weapon
    public void FireWeapon(bool localIsWhale, bool sentByRemote)
    {
        if (ammo > 0 && canFire)
        {
            MainGameHandler.bulletsFiredByPlayer.Add(MainGameHandler.CreateBullet(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.rotation, damage, bulletSpeed, sentByRemote, localIsWhale));
            ammo--;

            canFire = false;
        }
    }

    //reload weapon
    public void ReloadWeapon()
    {
        ammo = maxAmmo;
    }
}
