using System.Collections.Generic;
using UnityEngine;

public class ActiveAnimator : MonoBehaviour
{
    //define global variables
    public List<List<Sprite>> animationSets = new List<List<Sprite>>();
    public float interval = 0.75F;

    private float currentValue = 0;
    private int currentFrame = 0;
    private int currentSet = -1;

    //update
    void FixedUpdate()
    {
        if (currentSet != -1)
        {
            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            if (currentValue >= interval)
            {
                currentFrame++;
                if (currentFrame < animationSets[currentSet].Count) renderer.sprite = animationSets[currentSet][currentFrame];

                if (currentFrame > animationSets[currentSet].Count - 1)
                {
                    currentFrame = 0;
                    currentSet = -1;
                }

                currentValue = 0F;
            }

            currentValue += 0.1F;
        }
    }

    //play set
    public void PlaySet(int set)
    {
        if (currentSet == -1) currentSet = set;
    }
}
