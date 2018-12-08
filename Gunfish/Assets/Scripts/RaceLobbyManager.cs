using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceLobbyManager : MonoBehaviour {

    public static RaceLobbyManager instance;

	// Use this for initialization
	void Start () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        RaceManager.instance.InvokeLobbyTimer();
	}
}
