using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameHandler : MonoBehaviour {

    //sprites
    Sprite jellyFishSprite;
    Sprite whaleSprite;

    //player data
    public static GameObject player;
    bool isWhale = false;

    //game data

	// Use this for initialization
	void Start () {
        //load sprites
        jellyFishSprite = Resources.Load("jellyfish", typeof(Sprite)) as Sprite;

        //create player
        player = new GameObject("Player");
        player.AddComponent<PlayerControlScript>();
        player.AddComponent<BoxCollider2D>();
        Rigidbody2D body = player.AddComponent<Rigidbody2D>();

        body.isKinematic = false;
        body.gravityScale = 0;

        if (isWhale)
        {

        }
        else
        {
            SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = jellyFishSprite;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
