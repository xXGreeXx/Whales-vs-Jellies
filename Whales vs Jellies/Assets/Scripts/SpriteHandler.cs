using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHandler : MonoBehaviour {

    //define global variables
    public static Sprite moonJellySprite;
    public static Sprite moonJellySpriteAnim1;
    public static Sprite moonJellySpriteAnim2;
    public static Sprite moonJellySpriteAnim3;
    public static Sprite bottleNoseSprite;
    public static Sprite bottleNoseSpriteAnim1;
    public static Sprite bottleNoseSpriteAnim2;
    public static Sprite jellyfishSpineShooter;
    public static Sprite topHatSprite;
    public static Sprite cigarSprite;
    public static Sprite sunglassesSprite;
    public static Sprite visorSprite;
    public static Sprite bulletProofVestSprite;
    public static Sprite pirateHatSprite;
    public static Sprite bulletSprite;
    public static Sprite backgroundSprite;
    public static Sprite bubbleSpriteAnim1;
    public static Sprite bubbleSpriteAnim2;

    //start
    void Start ()
    {
        //load sprites
        moonJellySprite = Resources.Load("MoonJellyAnim/moonjelly0", typeof(Sprite)) as Sprite;
        moonJellySpriteAnim1 = Resources.Load("MoonJellyAnim/moonjelly1", typeof(Sprite)) as Sprite;
        moonJellySpriteAnim2 = Resources.Load("MoonJellyAnim/moonjelly2", typeof(Sprite)) as Sprite;
        moonJellySpriteAnim3 = Resources.Load("MoonJellyAnim/moonjelly3", typeof(Sprite)) as Sprite;
        bottleNoseSprite = Resources.Load("BottleNoseAnim/bottlenose0", typeof(Sprite)) as Sprite;
        bottleNoseSpriteAnim1 = Resources.Load("BottleNoseAnim/bottlenose1", typeof(Sprite)) as Sprite;
        bottleNoseSpriteAnim2 = Resources.Load("BottleNoseAnim/bottlenose2", typeof(Sprite)) as Sprite;
        jellyfishSpineShooter = Resources.Load("jellyfishSpineShooter", typeof(Sprite)) as Sprite;
        topHatSprite = Resources.Load("topHat", typeof(Sprite)) as Sprite;
        cigarSprite = Resources.Load("cigar", typeof(Sprite)) as Sprite;
        sunglassesSprite = Resources.Load("sunglasses", typeof(Sprite)) as Sprite;
        visorSprite = Resources.Load("visor", typeof(Sprite)) as Sprite;
        bulletProofVestSprite = Resources.Load("bulletProofVest", typeof(Sprite)) as Sprite;
        pirateHatSprite = Resources.Load("pirateHat", typeof(Sprite)) as Sprite;
        bulletSprite = Resources.Load("jellyfishSpine", typeof(Sprite)) as Sprite;
        backgroundSprite = Resources.Load("background", typeof(Sprite)) as Sprite;
        bubbleSpriteAnim1 = Resources.Load("BubbleAnim/bubble1", typeof(Sprite)) as Sprite;
        bubbleSpriteAnim2 = Resources.Load("BubbleAnim/bubble2", typeof(Sprite)) as Sprite;
    }
}
