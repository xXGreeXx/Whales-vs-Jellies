﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net.Sockets;
using System.IO;
using System.Text;

public class MainGameHandler : MonoBehaviour {
    //player data
    public static GameObject player;
    public static GameObject playerWeapon;
    public static List<GameObject> otherPlayers = new List<GameObject>();
    public static bool isWhale = false;
    public static int playerLevel = 0;
    public static int lastXPEarned = 0;

    //bullet data
    public static List<GameObject> bulletsFiredByPlayer = new List<GameObject>();
    public static List<GameObject> otherBullets = new List<GameObject>();

    //game data
    TcpClient clientInstance;
    public static String IP = "192.168.1.200";
    public static GameObject selectedItemInInventory;
    String whaleHealth = "20000/20000";
    String whaleLevel = "0";

    //ui data
    public static GameObject escapeMenuPanel;

	//start
	void Start ()
    {
        //find ui components
        escapeMenuPanel = GameObject.Find("EscapeMenu");
        escapeMenuPanel.SetActive(false);

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
            ParseData(data);
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

        //update whale health
        if (player.GetComponent<PlayerData>().isWhale)
        {
            whaleHealth = player.GetComponent<PlayerData>().health + "/" + player.GetComponent<PlayerData>().maxHealth;
        }
        GameObject.Find("WhaleHealthText").GetComponent<UnityEngine.UI.Text>().text = whaleHealth;

        //check if lost
        if (player.GetComponent<PlayerData>().health <= 0)
        {
            playerLevel += calculateXPGain();
            Disconnect();
            SceneManager.LoadScene("LossScreen");
        }
    }

    //update
    void Update()
    {
        //escape menu
        if (Input.GetKeyDown(KeyCode.Escape)) escapeMenuPanel.SetActive(!escapeMenuPanel.activeSelf);

        if (!escapeMenuPanel.activeSelf)
        {
            //handle weapon fire/rotate
            if (Input.GetMouseButtonDown(0))
            {
                playerWeapon.GetComponent<WeaponHandlerScript>().FireWeapon(100, 1F);
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

    //calculate xp earn
    private int calculateXPGain()
    {
        int xp = 5;

        lastXPEarned = xp;
        return xp;
    }

    //disconnect
    public static void Disconnect()
    {

    }

    //exit and save
    public static void ExitAndSave()
    {
        Application.Quit();
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
        writer.WriteLine(player.GetComponent<PlayerData>().isWhale.ToString());
        writer.Flush();
        writer.WriteLine(player.GetComponent<PlayerData>().health + "/" + player.GetComponent<PlayerData>().maxHealth);
        writer.Flush();
        writer.WriteLine(player.transform.Find("Weapon").transform.rotation.z);
        writer.Flush();

        //write bullet data
        foreach (GameObject bullet in bulletsFiredByPlayer)
        {
            if (bullet != null)
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
            else
            {
                break;
            }
        }
        writer.WriteLine("ENDOFBULLETS");
        writer.Flush();

        //write end
        writer.WriteLine("END");
        writer.Flush();

        //read data
        if (nwStream.DataAvailable)
        {
            String line;
            while (!(line = reader.ReadLine()).Equals("END"))
            {
                if (!line.Equals("")) data.Add(line);
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
            Debug.Log(index);

            //player data
            float x = float.Parse(data[index]);
            float y = float.Parse(data[index + 1]);
            float rot = float.Parse(data[index + 2]);
            Boolean localIsWhale = Boolean.Parse(data[index + 3]);
            String health = data[index + 4];
            float weaponRot = float.Parse(data[index + 5]);

            //set whale health
            if (localIsWhale) whaleHealth = health;

            if (playerIndex > otherPlayers.Count - 1)
            {
                GameObject p = CreatePlayer(localIsWhale);
                CreateWeapon(p);
                otherPlayers.Add(p);
            }

            //set extra data
            GameObject playerToEdit = otherPlayers[playerIndex];
            playerToEdit.transform.position = new Vector2(x, y);
            Rigidbody2D body = playerToEdit.GetComponent<Rigidbody2D>();
            body.rotation = rot;

            //(FAIL-SAFE) repurpose GOs for seperate players
            if (playerToEdit.GetComponent<PlayerData>().isWhale != localIsWhale)
            {
                Destroy(playerToEdit);
                playerToEdit = CreatePlayer(localIsWhale);
            }

            //continue setting extra data
            playerToEdit.transform.Find("Weapon").transform.rotation = new Quaternion(0, 0, weaponRot, 1);
            if (weaponRot < 0) weaponRot += 360;
            else if (weaponRot > 360) weaponRot -= 360;
            if ((weaponRot > 180 && weaponRot < 360) || (weaponRot < 0 && weaponRot > 180))
            {
                playerToEdit.transform.Find("Weapon").GetComponent<SpriteRenderer>().flipY = true;
            }
            else
            {
                playerToEdit.transform.Find("Weapon").GetComponent<SpriteRenderer>().flipY = false;
            }

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
            int tempIndexForBullets = 0;
            for (tempIndexForBullets = index + 6; tempIndexForBullets < data.Count; tempIndexForBullets += 5)
            {
                if (data[tempIndexForBullets].Equals("ENDOFBULLETS"))
                {
                    index += 1;
                    break;
                }

                float bulletX = float.Parse(data[tempIndexForBullets]);
                float bulletY = float.Parse(data[tempIndexForBullets + 1]);
                float bulletRot = float.Parse(data[tempIndexForBullets + 2]);
                float damage = float.Parse(data[tempIndexForBullets + 3]);
                float speed = float.Parse(data[tempIndexForBullets + 4]);

                if (bulletIndex > otherBullets.Count - 1)
                {
                    otherBullets.Add(CreateBullet(x, y, new Quaternion(0, 0, bulletRot, 1), damage, speed, true));
                }

                GameObject bulletToEdit = otherBullets[bulletIndex];
                bulletToEdit.transform.position = new Vector2(bulletX, bulletY);
                bulletToEdit.transform.rotation = new Quaternion(0, 0, bulletRot, 1);

                if (data[tempIndexForBullets + 5].Equals("ENDOFBULLETS"))
                {
                    index = tempIndexForBullets - 5;
                    break;
                }

                bulletIndex++;
            }

            playerIndex++;
        }

        //set player label
        GameObject.Find("playersText").GetComponent<UnityEngine.UI.Text>().text = (1 + playerIndex) + " Jellyfish";

        //remove extra players
        for (int tempIndex = playerIndex; tempIndex < otherPlayers.Count; tempIndex++)
        {
            RemovePlayer(tempIndex);
        }
    }

    //create bullet
    public static GameObject CreateBullet(float x, float y, Quaternion rot, float damage, float speed, Boolean sentByRemote)
    {
        GameObject bullet = new GameObject("Bullet");
        bullet.transform.position = new Vector2(x, y);

        if (isWhale) bullet.layer = 11;
        else bullet.layer = 10;

        SpriteRenderer renderer = bullet.AddComponent<SpriteRenderer>();
        BoxCollider2D collider = bullet.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.25F, 0.1F);
        BulletPhysicsScript physics = bullet.AddComponent<BulletPhysicsScript>();
        physics.canCollide = sentByRemote;
        Rigidbody2D body = bullet.AddComponent<Rigidbody2D>();

        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.gravityScale = 0;
        bullet.transform.rotation = rot;
        physics.bulletDamage = damage;
        physics.bulletSpeed = speed;
        renderer.sprite = SpriteHandler.bulletSprite;
        renderer.sortingOrder = -2;

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
            renderer.sprite = SpriteHandler.whaleSprite;
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
            renderer.sprite = SpriteHandler.jellyFishSprite;
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
        weaponRenderer.sprite = SpriteHandler.jellyfishSpineShooter;
        weaponRenderer.sortingOrder = -1;
        WeaponHandlerScript weaponData = weapon.AddComponent<WeaponHandlerScript>();
        weaponData.maxAmmo = 10;
        weaponData.ammo = weaponData.maxAmmo;

        return weapon;
    }
}
