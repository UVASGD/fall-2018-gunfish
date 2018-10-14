using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public static PlayerManager PM;
    public List<GameObject> playerList;

    // Use this for initialization
    void Start () {
        // Initialize this class as a singleton
        if (PM == null)
            PM = this;
        else
            Destroy(this);

        // Initialize a list of players
        playerList = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Function to add a new player to the list
    void AddPlayer(GameObject newPlayer) {
        playerList.Add(newPlayer);
    }
}
