using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ninja Runner Perfect Zone class
//Small class, used to see if the Player landed a perfect jump on a given platform

public class PerfectZone : MonoBehaviour {

    private PlatformGenerator.Platform  parent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && parent.perfected == false)
        {
            GameController.PerfectJump();

            parent.perfected = true;
        }
    }

    public void SetParent(PlatformGenerator.Platform pp)
    {
        parent = pp;
    }
}
