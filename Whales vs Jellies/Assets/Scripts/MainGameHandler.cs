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
    public static bool isWhale = true;

    //game data
    TcpClient clientInstance;
    public static String IP = "192.168.1.200";

	//start
	void Start ()
    {
        //TODO\\ remove this code it is temp
        if (File.Exists("whale.txt")) isWhale = true;

        //load sprites
        jellyFishSprite = Resources.Load("jellyfish", typeof(Sprite)) as Sprite;
        whaleSprite = Resources.Load("whale", typeof(Sprite)) as Sprite;

        //connect to server
        clientInstance = new TcpClient();
        try
        {
            clientInstance.Connect(IP, 8888);
            clientInstance.NoDelay = true;
        }
        catch (Exception) { SceneManager.LoadScene("MainMenu"); }

        //create player
        player = CreatePlayer(isWhale);
        player.AddComponent<PlayerControlScript>();

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

    //fixed update
    void FixedUpdate ()
    {
        //interface server
        if (IsConnected(clientInstance.Client))
        {
            List<String> data = ReadWriteServer();
            if (data.Count >= 4 ) ParseData(data);
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
        writer.WriteLine(isWhale);
        writer.Flush();
        writer.WriteLine("END");
        writer.Flush();

        if (nwStream.DataAvailable)
        {
            String line;
            while (!(line = reader.ReadLine()).Equals("END"))
            {
                float res;
                Boolean res2;
                if (float.TryParse(line, out res) || Boolean.TryParse(line, out res2)) data.Add(line);
            }
        }

        return data;
    }

    //parse data from server
    private void ParseData(List<String> data)
    {
        //handle player movement
        int playerIndex = 0;
        for (int index = 0; index < data.Count; index += 4)
        {
            float x = float.Parse(data[index]);
            float y = float.Parse(data[index + 1]);
            float rot = float.Parse(data[index + 2]);
            Boolean localIsWhale = Boolean.Parse(data[index + 3]);

            if (playerIndex > otherPlayers.Count - 1)
            {
                otherPlayers.Add(CreatePlayer(localIsWhale));
            }

            GameObject playerToEdit = otherPlayers[playerIndex];
            playerToEdit.transform.position = new Vector2(x, y);
            Rigidbody2D body = playerToEdit.GetComponent<Rigidbody2D>();
            body.rotation = rot;

            if (playerToEdit.GetComponent<SpriteRenderer>().sprite.Equals(whaleSprite))
            {
                SpriteRenderer renderer = playerToEdit.gameObject.GetComponent<SpriteRenderer>();

                if ((body.rotation > 180 && body.rotation < 360) || (body.rotation < 0 && body.rotation > 180))
                {
                    renderer.flipX = true;
                }
                else
                {
                    renderer.flipX = false;
                }
            }

            playerIndex++;
        }

        //remove extra players
        for (int tempIndex = playerIndex; tempIndex < otherPlayers.Count; tempIndex++)
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
    private GameObject CreatePlayer(Boolean localIsWhale)
    {
        GameObject p = new GameObject("Player");
        p.AddComponent<BoxCollider2D>();
        p.layer = 9;
        Rigidbody2D bodyBase = p.AddComponent<Rigidbody2D>();

        bodyBase.isKinematic = false;
        bodyBase.gravityScale = 0;
        SpriteRenderer renderer = p.AddComponent<SpriteRenderer>();

        if (localIsWhale)
        {
            p.transform.position = new Vector2(32.8F, -10);
            p.transform.localScale = new Vector3(3, 3, 1);
            bodyBase.rotation = 90;
            renderer.sprite = whaleSprite;
            renderer.sortingOrder = -2;
        }
        else
        {
            p.transform.position = new Vector2(UnityEngine.Random.Range(-39, -10), UnityEngine.Random.Range(2, -18));
            renderer.sprite = jellyFishSprite;
            renderer.sortingOrder = -2;
        }

        return p;
    }
}
