using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ninja Runner Player Land class
//Used (with Perfect Zone) to see if the Player has landed a perject jump

public class PlayerLand : MonoBehaviour {

    private PlatformGenerator.Platform parent;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            if (!parent.perfected)
            {
                GameController.perfectJumpMultiplier = 0;
            }
        }
    }

    public void SetParent(PlatformGenerator.Platform pp)
    {
        parent = pp;
    }
}
