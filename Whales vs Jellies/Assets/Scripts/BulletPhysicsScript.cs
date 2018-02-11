using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPhysicsScript : MonoBehaviour {
    //define global variables
    public float bulletSpeed = 1F;
    public float bulletDamage = 10F;
    public bool canCollide;

	// Update is called once per frame
	void FixedUpdate ()
    {
        //move bullet
        Rigidbody2D body = gameObject.transform.GetComponent<Rigidbody2D>();

        body.AddRelativeForce(new Vector2(bulletSpeed, 0), ForceMode2D.Impulse);
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (canCollide || (collision.gameObject.name.Equals("wall") || collision.gameObject.name.Equals("sand")))
        {
            if (collision.gameObject.name.Equals("Player1"))
            {
                MainGameHandler.player.GetComponent<PlayerData>().health -= bulletDamage;
            }

            if (MainGameHandler.otherBullets.Contains(gameObject)) MainGameHandler.otherBullets.Remove(gameObject);

            Destroy(gameObject, 0);
        }
    }
}
