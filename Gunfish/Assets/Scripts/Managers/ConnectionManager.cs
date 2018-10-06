using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectionManager : MonoBehaviour {

    public static ConnectionManager instance;

    public List<PlayerConnection> players;

	// Use this for initialization
	void Start () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SortByScore () {
        List<PlayerConnection> temp = new List<PlayerConnection>();

        for (int i = 0; i < players.Count; i++) {
            int highest = 0;
            int highestIndex = 0;
            for (int j = 0; j < players.Count; j++) {
                if (players[j].score > highest) {
                    highest = players[j].score;
                    highestIndex = j;
                }
            }
            temp.Add(players[highestIndex]);
        }
        players = temp;
    }
}
