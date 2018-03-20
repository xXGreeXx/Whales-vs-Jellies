using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {
    //global variables
    public Vector2 targetPosition = Vector2.zero;

    //handle most ai stuff in fixed update
    void FixedUpdate()
    {
        targetPosition = MainGameHandler.player.transform.position;

        Vector2 diff = targetPosition - (Vector2)gameObject.transform.position;
        diff.Normalize();
        float deg = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        float yVel = 0.25F;

        if (yVel != 0)
        {
            gameObject.GetComponent<ActiveAnimator>().PlaySet(0);
        }

        gameObject.transform.Find("Weapon").transform.rotation = Quaternion.Euler(new Vector3(0, 0, deg));
        Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();

        body.MoveRotation(deg - 90);
        if (body.rotation < 0) body.rotation += 360;
        else if (body.rotation > 360) body.rotation -= 360;
        body.AddRelativeForce(new Vector2(0, yVel / 3), ForceMode2D.Impulse);

        if (body.position.y > 3F)
        {
            body.AddForce(new Vector2(0, -20));
        }

        if (gameObject.GetComponent<PlayerData>().health <= 0)
        {
            MainGameHandler.AIs.Remove(gameObject);
            Destroy(gameObject, 0);
        }

        if (Random.Range(0, 400) == 1)
        {
            gameObject.transform.Find("Weapon").GetComponent<WeaponHandlerScript>().FireWeapon(false, true);
            gameObject.transform.Find("Weapon").GetComponent<WeaponHandlerScript>().ReloadWeapon();
        }
    }
}
