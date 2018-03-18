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
        Rigidbody2D body = gameObject.transform.GetComponent<Rigidbody2D>();
        if (body.velocity == Vector2.zero) Destroy(gameObject, 0);

        if (!canCollide)
        {
            MainGameHandler.CreateBubble(body.position.x, body.position.y);
        }
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (canCollide || (collision.gameObject.name.Equals("wall") || collision.gameObject.name.Equals("sand") || collision.gameObject.name.Equals("AI")))
        {
            if (collision.gameObject.name.Equals("Player1") || collision.gameObject.name.Equals("AI"))
            {
                collision.gameObject.GetComponent<PlayerData>().health -= bulletDamage;
            }

            if (MainGameHandler.otherBullets.Contains(gameObject)) MainGameHandler.otherBullets.Remove(gameObject);

            Destroy(gameObject, 0);
        }
    }
}
