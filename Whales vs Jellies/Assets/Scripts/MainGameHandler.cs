using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;

public class MainGameHandler : MonoBehaviour {
    //enums
    public enum CreatureTypes
    {
        MoonJelly,
        CannonballJelly,
        BluefireJelly,
        BoxJelly,
        BottleNose
    }

    public enum WeaponTypes
    {
        EMPTY,
        nematocyst,
        harpoonGun,
        eggBazooka
    }

    //player data
    public static GameObject player;
    public static GameObject playerWeapon;
    public static GameObject playerVest;
    public static GameObject playerHat;
    public static GameObject playerMouthPiece;
    public static GameObject playerEyePiece;
    public static bool isWhale = false;
    public static CreatureTypes type;
    public static int playerLevel = 1;
    public static int playerCurrentXP = 0;
    public static int xpNeededForNextLevel = 100;
    public static int currency = 0;
    public static float attack = 0;
    public static float defense = 0;
    public static float health = 0;
    public static float speed = 0;

    public static float damageDealtToWhale = 0;

    //player "cosmetic" data
    public static String weaponType = "harpoonGun";
    public static String vestType = "EMPTY";
    public static String hatType = "EMPTY";
    public static String mouthpieceType = "EMPTY";
    public static String eyepieceType = "EMPTY";

    //bullet data
    public static List<GameObject> bulletsFiredByPlayer = new List<GameObject>();
    public static List<GameObject> otherBullets = new List<GameObject>();

    //game data
    public static TcpClient clientInstance;
    public static String IP = "";
    public static GameObject selectedItemInInventory;
    String whaleHealth = "20000/20000";
    String whaleLevel = "0";
    public static List<GameObject> otherPlayers = new List<GameObject>();
    public static int lastXPEarned = 0;

    //single player game data
    public static List<GameObject> AIs = new List<GameObject>();
    private static int amountOfAIs = 100;

    //"cosmetic" maps
    public static Dictionary<String, Sprite> weaponSpritesMap = new Dictionary<string, Sprite>()
    {
        {"EMPTY", new Sprite() },
        { "nematocyst", SpriteHandler.nematocystSpriteLoaded },
        { "harpoonGun", SpriteHandler.harpoonGunSprite },
        { "eggBazooka", SpriteHandler.omeletteBringerSprite }
    };
    public static Dictionary<String, Sprite> hatSpritesMap = new Dictionary<string, Sprite>()
    {
        {"EMPTY", new Sprite() },
        { "topHat", SpriteHandler.topHatSprite },
        { "pirateHat", SpriteHandler.pirateHatSprite }
    };
    public static Dictionary<String, Sprite> eyepieceSpritesMap = new Dictionary<string, Sprite>()
    {
        {"EMPTY", new Sprite() },
        { "sunglasses", SpriteHandler.sunglassesSprite },
        { "visor", SpriteHandler.visorSprite },
    };
    public static Dictionary<String, Sprite> mouthpieceSpritesMap = new Dictionary<string, Sprite>()
    {
        {"EMPTY", new Sprite() },
        { "cigar", SpriteHandler.cigarSprite },
    };
    public static Dictionary<String, Sprite> vestSpritesMap = new Dictionary<string, Sprite>()
    {
        {"EMPTY", new Sprite() },
        { "bulletProofVest", SpriteHandler.bulletProofVestSprite },
    };

    //ui data
    public static GameObject escapeMenuPanel;

    //net code data
    public const int amountOfDataPerNewPlayer = 13;
    public const int amountOfDataPerUpdatePlayer = 6;
    private Boolean sentBaseData = false;

	//start
	void Start ()
    {
        //find ui components
        escapeMenuPanel = GameObject.Find("EscapeMenu");
        escapeMenuPanel.SetActive(false);

        //reset static variables
        bulletsFiredByPlayer = new List<GameObject>();
        otherBullets = new List<GameObject>();
        otherPlayers = new List<GameObject>();

        //connect to server
        if (!IP.Equals(String.Empty))
        {
            clientInstance = new TcpClient();
            try
            {
                clientInstance.Connect(IP, 8888);
                clientInstance.NoDelay = true;
            }
            catch (Exception) { SceneManager.LoadScene("MainMenu"); }
        }
        else
        {
            InitializeAI();
        }

        //create player
        player = CreatePlayer(type);
        player.name = "Player1";
        player.AddComponent<PlayerControlScript>();
        if (!weaponType.Equals(String.Empty)) playerWeapon = CreateWeapon(player, weaponType, type);
        if (!eyepieceType.Equals(String.Empty)) playerEyePiece = CreateGlasses(player, eyepieceType, type);
        if (!hatType.Equals(String.Empty)) playerHat = CreateHat(player, hatType, type);
        if (!mouthpieceType.Equals(String.Empty)) playerMouthPiece = CreateMouthpiece(player, mouthpieceType, type);
        if (!vestType.Equals(String.Empty)) playerVest = CreateVest(player, vestType, type);
    }

    //initialize AI
    private void InitializeAI()
    {
        foreach (GameObject a in AIs) Destroy(a, 0);
        AIs = new List<GameObject>();

        for (int i = 0; i < amountOfAIs; i++)
        {
            GameObject AIGO = CreatePlayer(CreatureTypes.MoonJelly);
            AIGO.name = "AI";

            PlayerData AIData = AIGO.AddComponent<PlayerData>();
            AI AICore = AIGO.AddComponent<AI>();

            AIData.health = 100;
            AIData.maxHealth = 100;

            CreateWeapon(AIGO, "nematocyst", CreatureTypes.MoonJelly);
            CreateMouthpiece(AIGO, "cigar", CreatureTypes.MoonJelly);
            CreateGlasses(AIGO, "sunglasses", CreatureTypes.MoonJelly);

            AIs.Add(AIGO);
        }
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
        if (!IP.Equals(String.Empty))
        {
            if (IsConnected(clientInstance.Client))
            {
                List<String> data = ReadWriteServer();
                String command;
                if (data.Count >= amountOfDataPerNewPlayer + 1) ParseData(data, out command);
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
        else
        {
            GameObject.Find("playersText").GetComponent<UnityEngine.UI.Text>().text = AIs.Count + " Jellyfish";
        }

        //update HUD labels
        GameObject.Find("healthText").GetComponent<UnityEngine.UI.Text>().text = player.GetComponent<PlayerData>().health + "/" + player.GetComponent<PlayerData>().maxHealth;
        GameObject.Find("ammoText").GetComponent<UnityEngine.UI.Text>().text = playerWeapon.GetComponent<WeaponHandlerScript>().ammo + "/" + playerWeapon.GetComponent<WeaponHandlerScript>().maxAmmo;

        //update whale health
        if (player.GetComponent<PlayerData>().isWhale)
        {
            whaleHealth = player.GetComponent<PlayerData>().health + "/" + player.GetComponent<PlayerData>().maxHealth;
            whaleLevel = Convert.ToString(playerLevel);
        }
        GameObject.Find("WhaleHealthText").GetComponent<UnityEngine.UI.Text>().text = whaleHealth;
        GameObject.Find("WhaleLevelText").GetComponent<UnityEngine.UI.Text>().text = "Level: " + whaleLevel;
        String whaleHealthLow = "0";
        String whaleHealthHigh = "0";
        int i = 0;
        foreach (char c in whaleHealth)
        {
            String key = c.ToString();
            if (key.Equals("/"))
            {
                whaleHealthHigh = whaleHealth.Substring(i + 1, whaleHealth.Length - (i + 1));
                break;
            }
            else
            {
                whaleHealthLow += c;
            }

            i++;
        }

        float whaleHealthLowParsed = float.Parse(whaleHealthLow);
        float whaleHealthHighParsed = float.Parse(whaleHealthHigh);
        GameObject whaleHealthForeground = GameObject.Find("WhaleHealthForeground");
        GameObject whaleHealthBackground = GameObject.Find("WhaleHealthBackground");

        float finalX = (whaleHealthLowParsed / whaleHealthHighParsed * whaleHealthBackground.GetComponent<RectTransform>().rect.width);

        whaleHealthForeground.GetComponent<RectTransform>().sizeDelta = new Vector2(finalX, whaleHealthBackground.GetComponent<RectTransform>().rect.height);

        //check if lost
        if (player.GetComponent<PlayerData>().health <= 0)
        {
            playerCurrentXP += calculateXPGain();
            currency += lastXPEarned;
            CheckIfLeveled();

            Disconnect();
            SceneManager.LoadScene("LossScreen");
        }

        //check if won
        if ((IP.Equals("") && AIs.Count <= 0)) //TODO include multiplayer win too
        {
            playerCurrentXP += calculateXPGain();
            currency += lastXPEarned;
            CheckIfLeveled();

            Disconnect();
            SceneManager.LoadScene("WinScreen");
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
            WeaponHandlerScript script = playerWeapon.GetComponent<WeaponHandlerScript>();

            if (script.fullyAuto)
            {
                if (Input.GetMouseButton(0))
                {
                    script.FireWeapon(isWhale, false);
                    if(script.canFire && !script.reloading) for (int i = 0; i < UnityEngine.Random.Range(3, 7); i++) CreateBubble(player.transform.position.x + (i / 5.5F) * UnityEngine.Random.Range(-0.5F, 0.5F), player.transform.position.y);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    script.FireWeapon(isWhale, false);
                    if(script.canFire && !script.reloading) for (int i = 0; i < UnityEngine.Random.Range(3, 7); i++) CreateBubble(player.transform.position.x + (i / 5.5F) * UnityEngine.Random.Range(-0.5F, 0.5F), player.transform.position.y);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                script.ReloadWeapon();
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

    //check if player leveled up
    public void CheckIfLeveled()
    {
        while (playerCurrentXP >= xpNeededForNextLevel)
        {
            playerCurrentXP -= xpNeededForNextLevel;
            playerLevel++;

            float unTruncatedXPForNextLevel = xpNeededForNextLevel * 1.35F;
            xpNeededForNextLevel = (int)Math.Round(unTruncatedXPForNextLevel);
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
        int xp = 1500;

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
        if (!sentBaseData)
        {
            String command = "n";
            writer.Flush();
            writer.WriteLine(command);
            writer.Flush();
            writer.WriteLine(player.transform.position.x);
            writer.Flush();
            writer.WriteLine(player.transform.position.y);
            writer.Flush();
            writer.WriteLine(player.transform.GetComponent<Rigidbody2D>().rotation);
            writer.Flush();
            writer.WriteLine(type.ToString());
            writer.Flush();
            writer.WriteLine(player.GetComponent<PlayerData>().health + "/" + player.GetComponent<PlayerData>().maxHealth);
            writer.Flush();
            writer.WriteLine(player.transform.Find("Weapon").transform.rotation.z);
            writer.Flush();
            writer.WriteLine(weaponType);
            writer.Flush();
            writer.WriteLine(vestType);
            writer.Flush();
            writer.WriteLine(hatType);
            writer.Flush();
            writer.WriteLine(mouthpieceType);
            writer.Flush();
            writer.WriteLine(eyepieceType);
            writer.Flush();
            writer.WriteLine(playerLevel);
            writer.Flush();

            sentBaseData = true;
        }
        else
        {
            String command = "u";
            writer.Flush();
            writer.WriteLine(command);
            writer.Flush();
            writer.WriteLine(player.transform.position.x);
            writer.Flush();
            writer.WriteLine(player.transform.position.y);
            writer.Flush();
            writer.WriteLine(player.transform.GetComponent<Rigidbody2D>().rotation);
            writer.Flush();
            writer.WriteLine(player.GetComponent<PlayerData>().health + "/" + player.GetComponent<PlayerData>().maxHealth);
            writer.Flush();
            writer.WriteLine(player.transform.Find("Weapon").transform.rotation.z);
            writer.Flush();
        }

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
                writer.WriteLine(bullet.transform.rotation.eulerAngles.z);
                writer.Flush();
                writer.WriteLine(physics.bulletDamage);
                writer.Flush();
                writer.WriteLine(physics.bulletSpeed);
                writer.Flush();
            }
            else
            {
                continue;
            }
        }
        writer.WriteLine("ENDOFBULLETS");
        writer.Flush();

        //write end
        writer.WriteLine("END");
        writer.Flush();

        //read data
        Stopwatch watch = Stopwatch.StartNew();

        if (nwStream.DataAvailable)
        {
            String line;
            while (!(line = reader.ReadLine()).Equals("END"))
            {
                if (!line.Equals("")) data.Add(line);

                if (line.Equals("") || line == null || watch.ElapsedMilliseconds >= 3000) //fail safe, this is hanging some times
                {
                    break;
                }
            }


        }
        watch.Stop();

        //update ping label
        GameObject.Find("pingText").GetComponent<UnityEngine.UI.Text>().text = watch.ElapsedMilliseconds / amountOfDataPerNewPlayer + "ms";

        return data;
    }

    //parse data from server
    private void ParseData(List<String> data, out String command)
    {
        command = "e"; //make compiler happy

        //handle data
        int jellyCount = 0;
        int playerIndex = 0;
        for (int index = 0; index < data.Count; index += 0)
        {
            //player data
            float x = 0;
            float y = 0;
            float rot = 0;
            String health = "/";
            float weaponRot = 0;

            command = data[index];
            UnityEngine.Debug.Log(command);
            if (command.Equals("n"))
            {
                x = float.Parse(data[index + 1]);
                y = float.Parse(data[index + 2]);
                rot = float.Parse(data[index + 3]);
                CreatureTypes localType = (CreatureTypes)Enum.Parse(typeof(CreatureTypes), data[index + 4]);
                health = data[index + 5];
                weaponRot = float.Parse(data[index + 6]);
                String localWeapon = data[index + 7];
                String localVest = data[index + 8];
                String localHat = data[index + 9];
                String localMouthpiece = data[index + 10];
                String localEyepiece = data[index + 11];
                int localLevel = int.Parse(data[index + 12]);

                GameObject p = CreatePlayer(localType);
                if (weaponSpritesMap.ContainsKey(localWeapon)) CreateWeapon(p, localWeapon, localType);
                if (vestSpritesMap.ContainsKey(localVest)) CreateVest(p, localVest, localType);
                if (hatSpritesMap.ContainsKey(localHat)) CreateHat(p, localHat, localType);
                if (mouthpieceSpritesMap.ContainsKey(localMouthpiece)) CreateMouthpiece(p, localMouthpiece, localType);
                if (eyepieceSpritesMap.ContainsKey(localEyepiece)) CreateGlasses(p, localEyepiece, localType);
                otherPlayers.Add(p);

                index += amountOfDataPerNewPlayer;
            }
            else if(command.Equals("u"))
            {
                x = float.Parse(data[index + 1]);
                y = float.Parse(data[index + 2]);
                rot = float.Parse(data[index + 3]);
                health = data[index + 4];
                weaponRot = float.Parse(data[index + 5]);

                index += amountOfDataPerUpdatePlayer;
            }

            //set extra data
            GameObject playerToEdit = otherPlayers[playerIndex];
            PlayerData playerInformation = playerToEdit.GetComponent<PlayerData>();

            Boolean localIsWhale = playerInformation.isWhale;
            if (localIsWhale)
            {
                whaleHealth = health;
                //whaleLevel = Convert.ToString(localLevel); //TODO\\
            }
            else jellyCount++;

            if (playerToEdit.transform.position != new Vector3(x, y, 0)) playerToEdit.GetComponent<ActiveAnimator>().PlaySet(0);

            playerToEdit.transform.position = new Vector2(x, y);
            Rigidbody2D body = playerToEdit.GetComponent<Rigidbody2D>();
            body.rotation = rot;

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
            for (tempIndexForBullets = index + amountOfDataPerNewPlayer; tempIndexForBullets < data.Count; tempIndexForBullets += 5)
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
                    otherBullets.Add(CreateBullet(x, y, Quaternion.Euler(0, 0, bulletRot), damage, speed, true, localIsWhale, (WeaponTypes)Enum.Parse(typeof(WeaponTypes), weaponType)));
                }

                GameObject bulletToEdit = otherBullets[bulletIndex];
                bulletToEdit.transform.position = new Vector2(bulletX, bulletY);
                bulletToEdit.transform.rotation = Quaternion.Euler(0, 0, bulletRot);

                if (data[tempIndexForBullets + 5].Equals("ENDOFBULLETS"))
                {
                    index = tempIndexForBullets;
                    break;
                }

                bulletIndex++;
            }

            playerIndex++;
        }

        //set player label
        GameObject.Find("playersText").GetComponent<UnityEngine.UI.Text>().text = (isWhale ? 0 : 1 + jellyCount) + " Jellyfish";

        //remove extra players //TODO\\
        //for (int tempIndex = playerIndex; tempIndex < otherPlayers.Count; tempIndex++)
        //{
        //    RemovePlayer(tempIndex);
        //}
    }

    //determine if whale
    public Boolean IsWhaleOrNot(CreatureTypes type)
    {
        if (type.Equals(CreatureTypes.BottleNose)) return true;
        else if (type.Equals(CreatureTypes.MoonJelly)) return false;

        return false; //make compiler happy
    }

    //create bubble
    public static void CreateBubble(float x, float y)
    {
        GameObject b = new GameObject("bubble");
        b.transform.position = new Vector2(x, y);
        b.transform.localScale = new Vector2(0.13F, 0.13F);
        b.AddComponent<BubbleScript>();
        SpriteRenderer renderer = b.AddComponent<SpriteRenderer>();

        BackgroundAnimator animator = b.AddComponent<BackgroundAnimator>();
        List<Sprite> set = new List<Sprite>();
        set.Add(SpriteHandler.bubbleSpriteAnim1);
        set.Add(SpriteHandler.bubbleSpriteAnim2);

        animator.framesOfAnimation = set;
        animator.random = true;
        animator.interval = 0.45F;

        renderer.sortingOrder = -3;
        renderer.sprite = SpriteHandler.bubbleSpriteAnim2;
    }

    //create bullet
    public static GameObject CreateBullet(float x, float y, Quaternion rot, float damage, float speed, Boolean sentByRemote, Boolean firedByWhale, WeaponTypes weaponUsed)
    {
        GameObject bullet = new GameObject("Bullet");
        bullet.transform.position = new Vector2(x, y);
        bullet.transform.localScale = new Vector2(0.5F, 0.5F);

        if (firedByWhale) bullet.layer = 11;
        else bullet.layer = 10;

        SpriteRenderer renderer = bullet.AddComponent<SpriteRenderer>();
        BoxCollider2D collider = bullet.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.25F, 0.1F);
        BulletPhysicsScript physics = bullet.AddComponent<BulletPhysicsScript>();
        physics.canCollide = sentByRemote;
        Rigidbody2D body = bullet.AddComponent<Rigidbody2D>();

        body.mass = 1F;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.gravityScale = 0;

        bullet.transform.rotation = rot;
        physics.bulletDamage = damage;
        if (weaponUsed.Equals(WeaponTypes.nematocyst)) renderer.sprite = SpriteHandler.jellySpineSprite;
        else if (weaponUsed.Equals(WeaponTypes.harpoonGun)) renderer.sprite = SpriteHandler.harpoonSprite;
        else if (weaponUsed.Equals(WeaponTypes.eggBazooka)) renderer.sprite = SpriteHandler.eggSprite;
        renderer.sortingOrder = -2;

        body.AddRelativeForce(new Vector2(speed, 0), ForceMode2D.Impulse);

        return bullet;
    }

    //remove player
    private void RemovePlayer(int index)
    {
        Destroy(otherPlayers[index], 1);
        otherPlayers.RemoveAt(index);
    }

    //create player
    private GameObject CreatePlayer(CreatureTypes localType)
    {
        GameObject p = new GameObject("Player");
        PlayerData data = p.AddComponent<PlayerData>();

        BoxCollider2D collider = p.AddComponent<BoxCollider2D>();
        p.layer = 9;
        Rigidbody2D bodyBase = p.AddComponent<Rigidbody2D>();

        bodyBase.isKinematic = false;
        bodyBase.gravityScale = 0;
        SpriteRenderer renderer = p.AddComponent<SpriteRenderer>();

        //bottlenose creation
        if (localType.Equals(CreatureTypes.BottleNose))
        {
            bodyBase.mass = 100;

            ActiveAnimator animator = p.AddComponent<ActiveAnimator>();
            animator.interval = 0.2F;

            List<Sprite> moveSet = new List<Sprite>();
            moveSet.Add(SpriteHandler.bottleNoseSprite);
            moveSet.Add(SpriteHandler.bottleNoseSpriteAnim1);
            moveSet.Add(SpriteHandler.bottleNoseSpriteAnim2);
            moveSet.Add(SpriteHandler.bottleNoseSpriteAnim3);
            moveSet.Add(SpriteHandler.bottleNoseSpriteAnim4);
            moveSet.Add(SpriteHandler.bottleNoseSpriteAnim3);
            moveSet.Add(SpriteHandler.bottleNoseSpriteAnim2);
            moveSet.Add(SpriteHandler.bottleNoseSpriteAnim1);
            moveSet.Add(SpriteHandler.bottleNoseSprite);

            animator.animationSets.Add(moveSet);

            collider.size = new Vector2(2.9F, 8F);
            data.maxHealth = 10000;
            data.health = data.maxHealth;
            data.moveSpeed = 30F;
            data.isWhale = IsWhaleOrNot(localType);

            p.transform.position = new Vector2(32.8F, -10);
            p.transform.localScale = new Vector3(0.25F, 0.25F, 1);
            p.layer = 8;
            bodyBase.rotation = 90;
            renderer.sprite = SpriteHandler.bottleNoseSprite;
            renderer.sortingOrder = -5;
        }
        //moonjelly creation
        else if(localType.Equals(CreatureTypes.MoonJelly))
        {
            bodyBase.mass = 5;

            //add active animator for jellyfish
            ActiveAnimator animator = p.AddComponent<ActiveAnimator>();
            List<Sprite> moveSet = new List<Sprite>();
            moveSet.Add(SpriteHandler.moonJellySprite);
            moveSet.Add(SpriteHandler.moonJellySpriteAnim1);
            moveSet.Add(SpriteHandler.moonJellySpriteAnim2);
            moveSet.Add(SpriteHandler.moonJellySpriteAnim3);
            moveSet.Add(SpriteHandler.moonJellySpriteAnim2);
            moveSet.Add(SpriteHandler.moonJellySpriteAnim1);
            moveSet.Add(SpriteHandler.moonJellySprite);

            animator.animationSets.Add(moveSet);

            //do other stuff
            p.transform.localScale = new Vector2(0.6F, 0.6F);

            collider.size = new Vector2(0.5F, 0.7F);
            data.maxHealth = 100;
            data.health = data.maxHealth;
            data.moveSpeed = 0.75F;
            data.isWhale = IsWhaleOrNot(localType);

            PhysicsMaterial2D mat = new PhysicsMaterial2D();
            mat.bounciness = 1;
            bodyBase.sharedMaterial = mat;

            p.transform.position = new Vector2(UnityEngine.Random.Range(-39, -10), UnityEngine.Random.Range(2, -18));
            renderer.sprite = SpriteHandler.moonJellySprite;
            renderer.sortingOrder = -5;
        }

        return p;
    }

    //create weapon
    private GameObject CreateWeapon(GameObject parent, String weaponType, CreatureTypes localType)
    {
        //create weapon
        GameObject weapon = new GameObject("Weapon");
        weapon.transform.SetParent(parent.transform);

        Vector3 attachPoint = Vector3.zero;
        Vector3 scalePoint = Vector3.zero;

        if (localType.Equals(CreatureTypes.MoonJelly)) { attachPoint = CustomizationHandler.moonjellyfishGunPoint; scalePoint = CustomizationHandler.moonjellyfishGunScale; }
        if (localType.Equals(CreatureTypes.BottleNose)) { attachPoint = CustomizationHandler.bottlenoseGunPoint; scalePoint = CustomizationHandler.bottlenoseGunScale; }

        if (IsWhaleOrNot(localType))
        {
            attachPoint /= 100;
            scalePoint /= 75;
            weapon.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            attachPoint /= 300;
            scalePoint /= 255;
        }

        weapon.transform.localPosition = attachPoint;
        weapon.transform.localScale = scalePoint;

        SpriteRenderer weaponRenderer = weapon.AddComponent<SpriteRenderer>();
        weaponRenderer.sprite = weaponSpritesMap[weaponType];
        weaponRenderer.sortingOrder = -3;

        WeaponHandlerScript weaponData = weapon.AddComponent<WeaponHandlerScript>();
        if (weaponType.Equals("nematocyst"))
        {
            weaponData.maxAmmo = 10;
            weaponData.ammo = weaponData.maxAmmo;
            weaponData.bulletSpeed = 40;
            weaponData.damage = 100;
            weaponData.firingSpeed = 8;
            weaponData.loadedSprite = SpriteHandler.nematocystSpriteLoaded;
            weaponData.unloadedSprite = SpriteHandler.nematocystSpriteUnloaded;
        }
        else if (weaponType.Equals("harpoonGun"))
        {
            weaponData.maxAmmo = 30;
            weaponData.ammo = weaponData.maxAmmo;
            weaponData.bulletSpeed = 30;
            weaponData.damage = 150;
            weaponData.firingSpeed = 2;
            weaponData.fullyAuto = true;
            weaponData.loadedSprite = SpriteHandler.harpoonGunSprite;
            weaponData.unloadedSprite = SpriteHandler.harpoonGunSprite;
        }
        else if (weaponType.Equals("eggBazooka"))
        {
            weaponData.maxAmmo = 1;
            weaponData.ammo = weaponData.maxAmmo;
            weaponData.bulletSpeed = 10;
            weaponData.damage = 500;
            weaponData.firingSpeed = 5;
            weaponData.fullyAuto = false;
            weaponData.loadedSprite = SpriteHandler.omeletteBringerSprite;
            weaponData.unloadedSprite = SpriteHandler.omeletteBringerSprite;
        }
        weaponData.type = (WeaponTypes)Enum.Parse(typeof(WeaponTypes), weaponType);

        return weapon;
    }

    //create hat
    private GameObject CreateHat(GameObject parent, String hatType, CreatureTypes localType)
    {
        //create weapon
        GameObject hat = new GameObject("Hat");
        hat.transform.SetParent(parent.transform);

        Vector3 attachPoint = Vector3.zero;
        Vector3 scalePoint = Vector3.zero;

        if (localType.Equals(CreatureTypes.MoonJelly)) { attachPoint = CustomizationHandler.moonjellyfishHatPoint; scalePoint = CustomizationHandler.moonjellyfishHatScale; }
        if (localType.Equals(CreatureTypes.BottleNose)) { attachPoint = CustomizationHandler.bottlenoseHatPoint; scalePoint = CustomizationHandler.bottlenoseHatScale; }

        if (IsWhaleOrNot(localType))
        {
            attachPoint /= 40;
            scalePoint /= 55;
            hat.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            attachPoint /= 275;
            scalePoint /= 200;
        }

        hat.transform.localPosition = attachPoint;
        hat.transform.localScale = scalePoint;

        SpriteRenderer hatRenderer = hat.AddComponent<SpriteRenderer>();
        hatRenderer.sprite = hatSpritesMap[hatType];
        hatRenderer.sortingOrder = -2;

        return hat;
    }

    //create glasses
    private GameObject CreateGlasses(GameObject parent, String glassesType, CreatureTypes localType)
    {
        //create weapon
        GameObject glasses = new GameObject("Glasses");
        glasses.transform.SetParent(parent.transform);

        Vector3 attachPoint = Vector3.zero;
        Vector3 scalePoint = Vector3.zero;

        if (localType.Equals(CreatureTypes.MoonJelly)) { attachPoint = CustomizationHandler.moonjellyfishEyePoint; scalePoint = CustomizationHandler.moonjellyfishEyeScale; }
        if (localType.Equals(CreatureTypes.BottleNose)) { attachPoint = CustomizationHandler.bottlenoseEyePoint; scalePoint = CustomizationHandler.bottlenoseEyeScale; }

        if (IsWhaleOrNot(localType))
        {
            attachPoint /= 40;
            scalePoint /= 50;
            glasses.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            attachPoint /= 325;
            scalePoint /= 200;
        }

        glasses.transform.localPosition = attachPoint;
        glasses.transform.localScale = scalePoint;

        SpriteRenderer hatRenderer = glasses.AddComponent<SpriteRenderer>();
        hatRenderer.sprite = eyepieceSpritesMap[glassesType];
        hatRenderer.sortingOrder = -3;

        return glasses;
    }

    //create mouthpiece
    private GameObject CreateMouthpiece(GameObject parent, String mouthpieceType, CreatureTypes localType)
    {
        //create weapon
        GameObject mouthpiece = new GameObject("Mouthpiece");
        mouthpiece.transform.SetParent(parent.transform);

        Vector3 attachPoint = Vector3.zero;
        Vector3 scalePoint = Vector3.zero;

        if (localType.Equals(CreatureTypes.MoonJelly)) { attachPoint = CustomizationHandler.moonjellyfishMouthPoint; scalePoint = CustomizationHandler.moonjellyfishMouthScale; }
        if (localType.Equals(CreatureTypes.BottleNose)) { attachPoint = CustomizationHandler.bottlenoseMouthPoint; scalePoint = CustomizationHandler.bottlenoseMouthScale; }

        if (IsWhaleOrNot(localType))
        {
            attachPoint /= 38;
            scalePoint /= 55;
            mouthpiece.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            attachPoint /= 450;
            scalePoint /= 200;
        }

        mouthpiece.transform.localPosition = attachPoint;
        mouthpiece.transform.localScale = scalePoint;

        SpriteRenderer hatRenderer = mouthpiece.AddComponent<SpriteRenderer>();
        hatRenderer.sprite = mouthpieceSpritesMap[mouthpieceType];
        hatRenderer.sortingOrder = -2;

        return mouthpiece;
    }

    //create vest
    private GameObject CreateVest(GameObject parent, String vestType, CreatureTypes localType)
    {
        //create weapon
        GameObject vest = new GameObject("Vest");
        vest.transform.SetParent(parent.transform);

        Vector3 attachPoint = Vector3.zero;
        Vector3 scalePoint = Vector3.zero;

        if (localType.Equals(CreatureTypes.MoonJelly)) { attachPoint = CustomizationHandler.moonjellyfishVestPoint; scalePoint = CustomizationHandler.moonjellyfishVestScale; }
        if (localType.Equals(CreatureTypes.BottleNose)) { attachPoint = CustomizationHandler.bottlenoseVestPoint; scalePoint = CustomizationHandler.bottlenoseVestScale; }

        if (IsWhaleOrNot(localType))
        {
            attachPoint /= 250;
            scalePoint /= 50;
        }
        else
        {
            attachPoint /= 275;
            scalePoint /= 200;
        }

        vest.transform.localPosition = attachPoint;
        vest.transform.localScale = scalePoint;

        SpriteRenderer hatRenderer = vest.AddComponent<SpriteRenderer>();
        hatRenderer.sprite = vestSpritesMap[vestType];
        hatRenderer.sortingOrder = -4;

        return vest;
    }
}
