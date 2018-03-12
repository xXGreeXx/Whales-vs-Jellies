using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPGainIncreaseScript : MonoBehaviour {
    //global variables
    public float IncreaseSpeed = 1.5F;
    public float IncreaseAmount = 5;
    private float currentTick = 0;
    private float currentXP = 0;

	// Update is called once per frame
	void FixedUpdate () {
        if (currentXP < MainGameHandler.lastXPEarned)
        {
            currentTick += 0.1F;

            if (currentTick >= IncreaseSpeed)
            {
                currentXP += IncreaseAmount;
                GetComponent<UnityEngine.UI.Text>().text = "XP Earned: " + currentXP;

                currentTick = 0;
            }
        }
	}
}
