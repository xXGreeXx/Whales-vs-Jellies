using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimator : MonoBehaviour {
    //define global variables
    public List<Sprite> framesOfAnimation = new List<Sprite>();
    public float interval = 1F;
    public bool random = false;

    private float currentValue = 0;
    private int currentFrame = 0;
	
	//update
	void FixedUpdate ()
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        if (currentValue >= interval)
        {
            if (!random)
            {
                currentFrame++;
                if (currentFrame < framesOfAnimation.Count) renderer.sprite = framesOfAnimation[currentFrame];

                if (currentFrame > framesOfAnimation.Count - 1)
                {
                    currentFrame = 0;
                }
            }
            else
            {
                renderer.sprite = framesOfAnimation[Random.Range(-1, framesOfAnimation.Count)];
            }

            currentValue = 0F;
        }

        currentValue += 0.1F;
	}
}
