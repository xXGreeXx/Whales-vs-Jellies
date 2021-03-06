﻿using UnityEngine;

public class PlayerControlScript : MonoBehaviour {
    //define global variables
    float xVel = 0;
    float yVel = 0;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!MainGameHandler.escapeMenuPanel.activeSelf)
        {
            if (Input.GetKey(KeyCode.W)) yVel = -MainGameHandler.player.GetComponent<PlayerData>().moveSpeed;
            else yVel = 0;
            if (Input.GetKey(KeyCode.A)) xVel = 0.1F;
            else if (Input.GetKey(KeyCode.D)) xVel = -0.1F;
            else xVel = 0;

            if (yVel != 0)
            {
                gameObject.GetComponent<ActiveAnimator>().PlaySet(0);
            }

            Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();

            body.MoveRotation(body.rotation + xVel * 20);
            if (body.rotation < 0) body.rotation += 360;
            else if (body.rotation > 360) body.rotation -= 360;
            body.AddRelativeForce(new Vector2(0, -yVel / 3), ForceMode2D.Impulse);
            body.drag = 1;
            Camera.main.transform.position = new Vector3(MainGameHandler.player.transform.position.x, MainGameHandler.player.transform.position.y, -10);

            if (body.position.y > 3F)
            {
                body.AddForce(new Vector2(0, -20));
            }

            if (MainGameHandler.isWhale)
            {
                SpriteRenderer renderer = body.gameObject.GetComponent<SpriteRenderer>();

                if ((body.rotation > 180 && body.rotation < 360) || (body.rotation < 0 && body.rotation > 180))
                {
                    //flip items
                    for (int child = 0; child < MainGameHandler.player.transform.childCount; child++)
                    {
                        GameObject childGO = MainGameHandler.player.transform.GetChild(child).gameObject;

                        if (!renderer.flipX)
                        {
                            childGO.transform.localPosition = Vector2.Scale(childGO.transform.localPosition, new Vector2(-1, 1));
                            childGO.GetComponent<SpriteRenderer>().flipY = true;
                        }
                    }

                    //flip whale
                    renderer.flipX = true;

                }
                else
                {
                    //flip items
                    for (int child = 0; child < MainGameHandler.player.transform.childCount; child++)
                    {
                        GameObject childGO = MainGameHandler.player.transform.GetChild(child).gameObject;

                        if (renderer.flipX)
                        {
                            childGO.transform.localPosition = Vector2.Scale(childGO.transform.localPosition, new Vector2(-1, 1));
                            childGO.GetComponent<SpriteRenderer>().flipY = false;
                        }
                    }

                    //flip whale
                    renderer.flipX = false;

                }
            }
        }
	}
}
