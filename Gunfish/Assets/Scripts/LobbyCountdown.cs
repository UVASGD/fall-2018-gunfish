using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyCountdown : NetworkBehaviour {

    public Text text;
    private int secondsRemaining;

	// Use this for initialization
	void Awake () {
        print("Register Request Time");
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.REQUESTTIME, OnRequestTime);
        print("Request Time Registered");
        InvokeRepeating("UpdateTime", 0f, 0.5f);
	}
	
	// Update is called once per frame
	//void Update () {
 //       int secondsRemaining = RaceManager.instance.secondsRemaining;
	//}

    [ServerCallback]
    void UpdateTime () {
        NetworkServer.SendToAll(MessageTypes.REQUESTTIME, new RequestTimeMsg(RaceManager.instance.secondsRemaining));
    }

    [ClientCallback]
    void OnRequestTime (NetworkMessage netMsg) {
        if (!text) return;

        print("Receiving message");

        RequestTimeMsg msg = netMsg.ReadMessage<RequestTimeMsg>();
        secondsRemaining = msg.time;
        text.text = (secondsRemaining > 0 ? secondsRemaining.ToString() : "GUNFISH");
        //text.fontSize = (secondsRemaining > 0 ? 60 : 120);
    }
}
