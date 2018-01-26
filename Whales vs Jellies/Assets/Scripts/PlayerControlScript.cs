using UnityEngine;

public class PlayerControlScript : MonoBehaviour {
    //define global variables
    float xVel = 0;
    float yVel = 0;

    float moveSpeed = 0.1F;
    float whaleMoveSpeed = 0.3F;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.W)) yVel = MainGameHandler.isWhale ? -whaleMoveSpeed : -moveSpeed;
        else yVel = 0;
        if (Input.GetKey(KeyCode.A)) xVel = moveSpeed;
        else if (Input.GetKey(KeyCode.D)) xVel = -moveSpeed;
        else xVel = 0;

        Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();

        body.MoveRotation(body.rotation + xVel * 20);
        body.AddRelativeForce(new Vector2(0, -yVel / 3), ForceMode2D.Impulse);
        body.drag = 1;
        Camera.main.transform.position = new Vector3(MainGameHandler.player.transform.position.x, MainGameHandler.player.transform.position.y, -10);

        if (body.position.y > 3F)
        {
            body.AddForce(new Vector2(0, -20));
        }
	}
}
