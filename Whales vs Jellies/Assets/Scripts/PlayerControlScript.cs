using UnityEngine;

public class PlayerControlScript : MonoBehaviour {
    //define global variables
    float xVel = 0;
    float yVel = 0;

    float moveSpeed = 0.1F;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.W)) yVel = -moveSpeed;
        else if (Input.GetKey(KeyCode.S)) yVel = moveSpeed;
        else yVel = 0;
        if (Input.GetKey(KeyCode.A)) xVel = moveSpeed;
        else if (Input.GetKey(KeyCode.D)) xVel = -moveSpeed;
        else xVel = 0;

        MainGameHandler.player.transform.position -= new Vector3(xVel, yVel, 0);
        Camera.main.transform.position = new Vector3(MainGameHandler.player.transform.position.x, MainGameHandler.player.transform.position.y, -10);
	}
}
