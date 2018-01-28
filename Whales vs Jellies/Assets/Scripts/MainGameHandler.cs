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
    Sprite jellyfishSpineShooter;
    public static Sprite bulletSprite;

    //player data
    public static GameObject player;
    public static GameObject playerWeapon;
    public static List<GameObject> otherPlayers = new List<GameObject>();
    public static bool isWhale = false;

    //bullet data
    public static List<GameObject> bulletsFiredByPlayer = new List<GameObject>();
    public static List<GameObject> otherBullets = new List<GameObject>();

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
        jellyfishSpineShooter = Resources.Load("jellyfishSpineShooter", typeof(Sprite)) as Sprite;
        bulletSprite = Resources.Load("jellyfishSpine", typeof(Sprite)) as Sprite;

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
        player.name = "Player1";
        player.AddComponent<PlayerControlScript>();
        playerWeapon = CreateWeapon(player);

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
            if (data.Count >= 6 ) ParseData(data);
        }
        else
        {
            clientInstance.GetStream().Close();
            clientInstance.Close();
            clientInstance = null;

            for (int index = 0; index < otherPlayers.Count; index++) RemovePlayer(index);

            SceneManager.LoadScene("MainMenu");
        }

        //update HUD labels
        GameObject.Find("healthText").GetComponent<UnityEngine.UI.Text>().text = player.GetComponent<PlayerData>().health + "/" + player.GetComponent<PlayerData>().maxHealth;
        GameObject.Find("ammoText").GetComponent<UnityEngine.UI.Text>().text = playerWeapon.GetComponent<WeaponHandlerScript>().ammo + "/" + playerWeapon.GetComponent<WeaponHandlerScript>().maxAmmo;
    }

    //update
    void Update()
    {
        //handle weapon fire/rotate
        if (Input.GetMouseButtonDown(0))
        {
            playerWeapon.GetComponent<WeaponHandlerScript>().FireWeapon(100);
        }
        if (Input.GetMouseButtonDown(1))
        {
            playerWeapon.GetComponent<WeaponHandlerScript>().ReloadWeapon();
        }

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
        playerWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);

        SpriteRenderer renderer = playerWeapon.GetComponent<SpriteRenderer>();
        if (angle < 0) angle += 360;
        else if (angle > 360) angle -= 360;
        if ((angle > 180 && angle < 360) || (angle < 0 && angle > 180))
        {
            renderer.flipY = true;
        }
        else
        {
            renderer.flipY = false;
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

        //write player data
        writer.Flush();
        writer.WriteLine(player.transform.position.x);
        writer.Flush();
        writer.WriteLine(player.transform.position.y);
        writer.Flush();
        writer.WriteLine(player.transform.GetComponent<Rigidbody2D>().rotation);
        writer.Flush();
        writer.WriteLine(player.GetComponent<PlayerData>().isWhale);
        writer.Flush();
        writer.WriteLine(player.GetComponent<PlayerData>().health);
        writer.Flush();
        writer.WriteLine(player.transform.Find("Weapon").transform.rotation.z);
        writer.Flush();

        //write bullet data
        foreach (GameObject bullet in bulletsFiredByPlayer)
        {
            BulletPhysicsScript physics = bullet.GetComponent<BulletPhysicsScript>();

            writer.WriteLine(bullet.transform.position.x);
            writer.Flush();
            writer.WriteLine(bullet.transform.position.y);
            writer.Flush();
            writer.WriteLine(bullet.transform.rotation.z);
            writer.Flush();
            writer.WriteLine(physics.bulletDamage);
            writer.Flush();
            writer.WriteLine(physics.bulletSpeed);
            writer.Flush();
        }

        //write end
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
        //handle data
        int playerIndex = 0;
        for (int index = 0; index < data.Count; index += 6)
        {
            //player data
            float x = float.Parse(data[index]);
            float y = float.Parse(data[index + 1]);
            float rot = float.Parse(data[index + 2]);
            Boolean localIsWhale = Boolean.Parse(data[index + 3]);
            float health = float.Parse(data[index + 4]);
            float weaponRot = float.Parse(data[index + 5]);

            if (playerIndex > otherPlayers.Count - 1)
            {
                GameObject p = CreatePlayer(localIsWhale);
                CreateWeapon(p);
                otherPlayers.Add(p);
            }

            GameObject playerToEdit = otherPlayers[playerIndex];
            playerToEdit.transform.position = new Vector2(x, y);
            Rigidbody2D body = playerToEdit.GetComponent<Rigidbody2D>();
            body.rotation = rot;
            playerToEdit.transform.Find("Weapon").transform.rotation = new Quaternion(0, 0, weaponRot, 1);

            if (playerToEdit.GetComponent<PlayerData>().isWhale)
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

            //handle bullet data
            int bulletIndex = 0;
            for (int i = index + 6; index < data.Count; index += 5)
            {
                float bulletX = float.Parse(data[i]);
                float bulletY = float.Parse(data[i + 1]);
                float bulletRot = float.Parse(data[i + 2]);
                float damage = float.Parse(data[i + 3]);
                float speed = float.Parse(data[i + 4]);

                if (bulletIndex > otherBullets.Count - 1)
                {
                    otherBullets.Add(CreateBullet(x, y, new Quaternion(0, 0, bulletRot, 1), damage, speed));
                }

                GameObject bulletToEdit = otherBullets[bulletIndex];
                bulletToEdit.transform.position = new Vector2(bulletX, bulletY);
                bulletToEdit.transform.rotation = new Quaternion(0, 0, bulletRot, 1);

                BulletPhysicsScript physics = bulletToEdit.GetComponent<BulletPhysicsScript>();
                physics.bulletDamage = damage;
                physics.bulletSpeed = speed;

                if (data[i + 6].Equals("END"))
                {
                    index = i + 6;
                    break;
                }

                bulletIndex++;
            }

            playerIndex++;
        }

        //remove extra players
        for (int tempIndex = playerIndex; tempIndex < otherPlayers.Count; tempIndex++)
        {
            RemovePlayer(tempIndex);
        }
    }

    //create bullet
    public static GameObject CreateBullet(float x, float y, Quaternion rot, float damage, float speed)
    {
        GameObject bullet = new GameObject("Bullet");
        bullet.transform.position = new Vector2(x, y);

        if (MainGameHandler.isWhale) bullet.layer = 11;
        else bullet.layer = 10;

        SpriteRenderer renderer = bullet.AddComponent<SpriteRenderer>();
        BoxCollider2D collider = bullet.AddComponent<BoxCollider2D>();
        BulletPhysicsScript physics = bullet.AddComponent<BulletPhysicsScript>();
        Rigidbody2D body = bullet.AddComponent<Rigidbody2D>();

        body.gravityScale = 0;
        body.transform.rotation = rot;
        physics.bulletDamage = damage;
        physics.bulletSpeed = 10F;
        renderer.sprite = bulletSprite;

        return bullet;
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
        PlayerData data = p.AddComponent<PlayerData>();

        BoxCollider2D collider = p.AddComponent<BoxCollider2D>();
        p.layer = 9;
        Rigidbody2D bodyBase = p.AddComponent<Rigidbody2D>();

        bodyBase.isKinematic = false;
        bodyBase.gravityScale = 0;
        SpriteRenderer renderer = p.AddComponent<SpriteRenderer>();

        data.isWhale = localIsWhale;

        if (localIsWhale)
        {
            collider.size = new Vector2(0.5F, 1F);
            data.maxHealth = 20000;
            data.health = data.maxHealth;
            data.moveSpeed = 0.3F;

            p.transform.position = new Vector2(32.8F, -10);
            p.transform.localScale = new Vector3(3, 3, 1);
            p.layer = 8;
            bodyBase.rotation = 90;
            renderer.sprite = whaleSprite;
            renderer.sortingOrder = -2;
        }
        else
        {
            collider.size = new Vector2(0.5F, 0.7F);
            data.maxHealth = 100;
            data.health = data.maxHealth;
            data.moveSpeed = 0.15F;

            PhysicsMaterial2D mat = new PhysicsMaterial2D();
            mat.bounciness = 1;
            bodyBase.sharedMaterial = mat;
            p.transform.position = new Vector2(UnityEngine.Random.Range(-39, -10), UnityEngine.Random.Range(2, -18));
            renderer.sprite = jellyFishSprite;
            renderer.sortingOrder = -2;
        }

        return p;
    }

    //create bullet
    private GameObject CreateWeapon(GameObject parent)
    {
        //create weapon
        GameObject weapon = new GameObject("Weapon");
        weapon.transform.SetParent(parent.transform);
        weapon.transform.position = new Vector2(parent.transform.position.x, parent.transform.position.y + 0.2F);
        weapon.transform.localScale = new Vector2(0.5F, 0.5F);
        SpriteRenderer weaponRenderer = weapon.AddComponent<SpriteRenderer>();
        weaponRenderer.sprite = jellyfishSpineShooter;
        weaponRenderer.sortingOrder = -1;
        WeaponHandlerScript weaponData = weapon.AddComponent<WeaponHandlerScript>();
        weaponData.maxAmmo = 10;
        weaponData.ammo = weaponData.maxAmmo;

        return weapon;
    }
}
