using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScript : MonoBehaviour {

	//update
	void FixedUpdate ()
    {
        gameObject.transform.position += new Vector3(Random.Range(-0.005F, 0.005F), 0.05F, 0);

        if (gameObject.transform.position.y > 2)
        {
            Destroy(gameObject);
        }
	}
}
