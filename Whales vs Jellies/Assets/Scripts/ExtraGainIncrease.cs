using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraGainIncrease : MonoBehaviour {
    //global variables
    public float IncreaseSpeed = 1.5F;
    public float IncreaseAmount = 5;
    private float currentTick = 0;
    private float currentValue = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (MainGameHandler.isWhale)
        {
            if (MainGameHandler.IP.Equals(string.Empty) ? currentValue < 100 - MainGameHandler.AIs.Count : currentValue < 100 - MainGameHandler.otherPlayers.Count)
            {
                currentTick += 0.1F;

                if (currentTick >= IncreaseSpeed)
                {
                    GetComponent<UnityEngine.UI.Text>().text = "Jellies Killed: " + currentValue;

                    currentValue += IncreaseAmount;
                    currentTick = 0;
                }
            }
            else
            {
                currentValue = MainGameHandler.IP.Equals(string.Empty) ? 100 - MainGameHandler.AIs.Count : 100 - MainGameHandler.otherPlayers.Count;
            }
        }
        else
        {
            if (currentValue < MainGameHandler.damageDealtToWhale)
            {
                currentTick += 0.1F;

                if (currentTick >= IncreaseSpeed)
                {
                    GetComponent<UnityEngine.UI.Text>().text = "Damage Dealt: " + currentValue;

                    currentValue += IncreaseAmount;
                    currentTick = 0;
                }
            }
            else
            {
                currentValue = MainGameHandler.damageDealtToWhale;
            }
        }
    }
}
