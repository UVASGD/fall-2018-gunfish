using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EndLevelText : NetworkBehaviour {
    public List<string> textOptions;

    public void Awake () {
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.REQUESTENDTEXT, DisplayEndText);
        print("Registered!");
    }

    public void DisplayEndText (NetworkMessage netMsg) {
        RequestEndTextMsg msg = netMsg.ReadMessage<RequestEndTextMsg>();

        GetComponent<Text>().text = (msg.text == "" ? textOptions[Random.Range(0, textOptions.Count)] : "FIN");
    }
}
