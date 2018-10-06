using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameModeHUD : MonoBehaviour {

    public string gameMode;

	// Use this for initialization
	void Start () {
        //gameMode += "GameMode";
	}
	
	// Update is called once per frame
	void Update () {
        NetworkManager.singleton.onlineScene = gameMode;
	}
}
