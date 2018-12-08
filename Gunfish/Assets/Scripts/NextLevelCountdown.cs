using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NextLevelCountdown : NetworkBehaviour {

    public static NextLevelCountdown instance;

    public Text text;
    private int secondsRemaining = 10;

	// Use this for initialization
	void Awake () {
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.REQUESTTIME, OnRequestTime);
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
            if (GameObject.FindWithTag("Timer") == null) {
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
