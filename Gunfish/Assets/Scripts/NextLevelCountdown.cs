using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NextLevelCountdown : NetworkBehaviour {

    public static NextLevelCountdown instance;

    static bool registered;

    public Text text;
    private int secondsRemaining = 10;

	// Use this for initialization
	void Awake () {
        if (!registered) {
            NetworkManager.singleton.client.RegisterHandler(MessageTypes.REQUESTTIME, OnRequestTime);
            registered = true;
        }
    }

    private void Start () {
        //if (!instance) {
        //    instance = this;
        //} else {
        //    Destroy(this);
        //}

        //DontDestroyOnLoad(this);
    }

    [ClientCallback]
    void OnRequestTime (NetworkMessage netMsg) {
        if (!text) {
            if (!GameObject.FindWithTag("Timer").GetComponent<Text>()) {
                return;
            } else {
                text = GameObject.FindWithTag("Timer").GetComponent<Text>();
            }
        }

        RequestTimeMsg msg = netMsg.ReadMessage<RequestTimeMsg>();
        secondsRemaining = msg.time;
        //text.text = (secondsRemaining > 0 ? secondsRemaining.ToString() : "GUNFISH");
        text.text = secondsRemaining.ToString();
    }
}
