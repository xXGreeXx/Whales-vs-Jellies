using UnityEngine;

public class WeaponHandlerScript : MonoBehaviour {
    //define global variables
    public int ammo;
    public int maxAmmo;
    public int damage;
    public float firingSpeed;
    public float bulletSpeed;
    public bool fullyAuto = false;

    public MainGameHandler.WeaponTypes type;

    private float round = 0;
    public bool canFire = true;
    public bool reloading = false;

    //fixed update
    void FixedUpdate()
    {
        if(!canFire) round += 0.3F;
        if (round >= firingSpeed)
        {
            round = 0;
            canFire = true;
        }

        if (reloading)
        {
            ammo++;
            if (ammo == maxAmmo)
            {
                reloading = false;
            }
        }
    }

    //fire weapon
    public void FireWeapon(bool localIsWhale, bool sentByRemote)
    {
        if (ammo > 0 && canFire && !reloading)
        {
            MainGameHandler.bulletsFiredByPlayer.Add(MainGameHandler.CreateBullet(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.rotation, damage, bulletSpeed, sentByRemote, localIsWhale, type));
            ammo--;

            canFire = false;
        }
    }

    //reload weapon
    public void ReloadWeapon()
    {
        reloading = true;
    }
}
