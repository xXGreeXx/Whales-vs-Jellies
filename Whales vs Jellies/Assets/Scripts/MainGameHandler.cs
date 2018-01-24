using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net.Sockets;
using System.IO;
using System.Text;

public class MainGameHandler : MonoBehaviour {

    //sprites
    Sprite jellyFishSprite;
    Sprite whaleSprite;

    //player data
    public static GameObject player;
    public static GameObject player2;
    bool isWhale = false;

    //game data
    TcpClient clientInstance;

	// Use this for initialization
	void Start ()
    {
        //load sprites
        jellyFishSprite = Resources.Load("jellyfish", typeof(Sprite)) as Sprite;

        //connect to server
        clientInstance = new TcpClient();
        try
        {
            clientInstance.Connect("172.30.26.177", 8888);
        }
        catch (Exception) { SceneManager.LoadScene("MainMenu"); }

        //create player
        player = new GameObject("Player1");
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

    //close socket with server on exit
    void OnApplicationQuit()
    {
        clientInstance.Close();
    }

    // Update is called once per frame
    void Update ()
    {
        //interface server
        String data = WriteThenReadFromServer();
        if (!data.Equals("")) ParseData(data);


    }

    //send data to server
    private String WriteThenReadFromServer()
    {
        String data = "";

        NetworkStream nwStream = clientInstance.GetStream();
        byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(player.transform.position.x + ", " + player.transform.position.y + ", " + player.transform.GetComponent<Rigidbody2D>().rotation + ", ");

        //read
        if (nwStream.DataAvailable)
        {
            byte[] bytesToRead = new byte[clientInstance.ReceiveBufferSize];
            int bytesRead = nwStream.Read(bytesToRead, 0, clientInstance.ReceiveBufferSize);
            data = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
        }

        //send
        nwStream.Write(bytesToSend, 0, bytesToSend.Length);

        return data;
    }

    //parse data from server
    private void ParseData(String data)
    {
        //parse data
        int x = 0;
        int y = 0;
        int rot = 0;
        String curSet = "x";
        String word = "";

        data = data.Replace(" ", "");
        for (int index = 0; index < data.Length; index++)
        {
            String c = data.Substring(index, 1);

            if (c.Equals(","))
            {
                if (curSet.Equals("x"))
                {
                    x = int.Parse(word);
                    curSet = "y";
                }
                else if (curSet.Equals("y"))
                {
                    y = int.Parse(word);
                    curSet = "rot";
                }
                else
                {
                    rot = int.Parse(word);
                }


                word = "";
            }
            else
            {
                word += c;
            }
        }

        //create player
        if (player2.Equals(null))
        {
            player2 = new GameObject("Player2");
            player2.AddComponent<PlayerControlScript>();
            player2.AddComponent<BoxCollider2D>();
            Rigidbody2D bodyBase = player2.AddComponent<Rigidbody2D>();

            bodyBase.isKinematic = false;
            bodyBase.gravityScale = 0;

            if (isWhale)
            {

            }
            else
            {
                SpriteRenderer renderer = player2.AddComponent<SpriteRenderer>();
                renderer.sprite = jellyFishSprite;
            }
        }

        player2.transform.position = new Vector2(x, y);
        Rigidbody2D body = player2.GetComponent<Rigidbody2D>();

        body.rotation = rot;
    }
}
