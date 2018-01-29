using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHandler : MonoBehaviour {

    //define global variables
    public static Sprite jellyFishSprite;
    public static Sprite whaleSprite;
    public static Sprite jellyfishSpineShooter;
    public static Sprite topHatSprite;
    public static Sprite cigarSprite;
    public static Sprite bulletSprite;
    public static Sprite backgroundSprite;

    //start
    void Start ()
    {
        //load sprites
        jellyFishSprite = Resources.Load("jellyfish", typeof(Sprite)) as Sprite;
        whaleSprite = Resources.Load("whale", typeof(Sprite)) as Sprite;
        jellyfishSpineShooter = Resources.Load("jellyfishSpineShooter", typeof(Sprite)) as Sprite;
        topHatSprite = Resources.Load("topHat", typeof(Sprite)) as Sprite;
        cigarSprite = Resources.Load("cigar", typeof(Sprite)) as Sprite;
        bulletSprite = Resources.Load("jellyfishSpine", typeof(Sprite)) as Sprite;
        backgroundSprite = Resources.Load("background", typeof(Sprite)) as Sprite;
    }
}
