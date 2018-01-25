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
    public static List<GameObject> otherPlayers = new List<GameObject>();
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
            renderer.sortingOrder = -2;
        }

        //create nametag
        GameObject nameTag = new GameObject("NameTag");
        nameTag.transform.SetParent(player.transform);
        UnityEngine.UI.Text text = nameTag.AddComponent<UnityEngine.UI.Text>();

        text.text = "Player1";
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
            if (data.Count >= 3 ) ParseData(data);
        }
        else
        {
            clientInstance.GetStream().Close();
            clientInstance.Close();
            clientInstance = null;

            for (int index = 0; index < otherPlayers.Count; index++) RemovePlayer(index);

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
                float res;
                if (float.TryParse(line, out res)) data.Add(line);

                Debug.Log(line);
            }
        }

        return data;
    }

    //parse data from server
    private void ParseData(List<String> data)
    {
        //handle player movement
        int playerIndex = 0;
        for (int index = 0; index < data.Count; index += 3)
        {
            float x = float.Parse(data[index]);
            float y = float.Parse(data[index + 1]);
            float rot = float.Parse(data[index + 2]);

            if (playerIndex > otherPlayers.Count - 1)
            {
                otherPlayers.Add(CreatePlayer());
            }

            GameObject playerToEdit = otherPlayers[playerIndex];
            playerToEdit.transform.position = new Vector2(x, y);
            Rigidbody2D body = playerToEdit.GetComponent<Rigidbody2D>();
            body.rotation = rot;

            playerIndex++;
        }

        //remove extra players
        for (int tempIndex = playerIndex; tempIndex < otherPlayers.Count - 1; tempIndex++)
        {
            RemovePlayer(tempIndex);
        }

    }

    //remove player
    private void RemovePlayer(int index)
    {
        Destroy(otherPlayers[index], 1);
        otherPlayers.RemoveAt(index);
    }

    //create player
    private GameObject CreatePlayer()
    {
        GameObject p = new GameObject("Player");
        p.AddComponent<BoxCollider2D>();
        p.layer = 9;
        Rigidbody2D bodyBase = p.AddComponent<Rigidbody2D>();

        bodyBase.isKinematic = false;
        bodyBase.gravityScale = 0;

        if (isWhale)
        {

        }
        else
        {
            SpriteRenderer renderer = p.AddComponent<SpriteRenderer>();
            renderer.sprite = jellyFishSprite;
            renderer.sortingOrder = -2;
        }

        return p;
    }
}
