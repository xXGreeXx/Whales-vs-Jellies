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
        player.layer = 9;
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
        if (IsConnected(clientInstance.Client))
        {
            List<String> data = ReadWriteServer();
            if (data.Count >= 3) ParseData(data);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }

    }

    //check if still connected
    public bool IsConnected(Socket s)
    {
        bool part1 = s.Poll(1000, SelectMode.SelectRead);
        bool part2 = (s.Available == 0);
        if (part1 && part2)
            return false;
        else
            return true;
    }

    //send data to server
    private List<String> ReadWriteServer()
    {
        List<String> data = new List<String>();

        NetworkStream nwStream = clientInstance.GetStream();
        StreamWriter writer = new StreamWriter(nwStream);
        StreamReader reader = new StreamReader(writer.BaseStream);

        writer.Flush();
        writer.WriteLine(player.transform.position.x);
        writer.Flush();
        writer.WriteLine(player.transform.position.y);
        writer.Flush();
        writer.WriteLine(player.transform.GetComponent<Rigidbody2D>().rotation);
        writer.Flush();
        writer.WriteLine("END");
        writer.Flush();

        if (nwStream.DataAvailable)
        {
            String line;
            while (!(line = reader.ReadLine()).Equals("END"))
            {
                data.Add(line);
            }
        }

        return data;
    }

    //parse data from server
    private void ParseData(List<String> data)
    {
        //create player
        if (GameObject.Find("Player2") == null)
        {
            player2 = new GameObject("Player2");
            player2.AddComponent<BoxCollider2D>();
            player2.layer = 9;
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

        player2.transform.position = new Vector2(float.Parse(data[0]), float.Parse(data[1]));
        Rigidbody2D body = player2.GetComponent<Rigidbody2D>();

        body.rotation = float.Parse(data[2]);
    }
}
