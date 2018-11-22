using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyCountdown : NetworkBehaviour {

    public Text text;
    private int secondsRemaining = 10;

	// Use this for initialization
	void Awake () {
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.REQUESTTIME, OnRequestTime);
    }

    [ClientCallback]
    void OnRequestTime (NetworkMessage netMsg) {
        if (!text) return;

        RequestTimeMsg msg = netMsg.ReadMessage<RequestTimeMsg>();
        secondsRemaining = msg.time;
        //text.text = (secondsRemaining > 0 ? secondsRemaining.ToString() : "GUNFISH");
        text.text = secondsRemaining.ToString();
    }
}
