using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientReset : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MusicManager.instance.PlayMusic();

        if (RaceManager.instance) {
            Destroy(RaceManager.instance.gameObject);
        }

        if (GameObject.Find("PlayerController")) {
            Destroy(GameObject.Find("PlayerController"));
        }
	}
}
