using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyManager : MonoBehaviour {

    private string playerName;
    private GameObject[] gunfishList;
    private int gunfishIndex;

    public int playerCount = 0;
    private int maxPlayerCount = 4;

	// Use this for initialization
	void Awake () {
        gunfishList = GunfishList.Get();
        gunfishIndex = 0;


	}

    private void OnPlayerConnected (NetworkIdentity player) {
        playerCount++;
        if (playerCount == maxPlayerCount) {
            StartGame();
        }
    }


    public void StartGame () {


    }
    //public override 

    // Update is called once per frame
    void Update () {
		
	}
}
