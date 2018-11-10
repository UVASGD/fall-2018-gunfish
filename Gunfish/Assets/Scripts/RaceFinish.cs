﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//SERVER ONLY
public class RaceFinish : NetworkBehaviour {

    static List<Gunfish> finishedFish = new List<Gunfish>();

    [ServerCallback]
    //When player collides with the finish box, this method is called
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Gunfish")) {
            Gunfish gunfish = other.gameObject.GetComponentInParent<Gunfish>();

            foreach (var fish in finishedFish) {
                if (fish == gunfish) return;
            }

            print("Other: " + other.transform.name);

            finishedFish.Add(gunfish);
            RaceManager.instance.PlayerFinish(gunfish);
        }
    }
}