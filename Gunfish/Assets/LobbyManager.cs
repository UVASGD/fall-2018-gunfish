using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour {

    private string playerName;
    private GameObject[] gunfishList;
    private int gunfishIndex;

	// Use this for initialization
	void Awake () {
        gunfishList = GunfishList.Get();
        gunfishIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
