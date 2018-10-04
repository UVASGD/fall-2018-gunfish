using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    private Text addrString;
    private Text portString;

    Button joinButton;
    Button hostButton;

    public string Addr
    {
        get {
            if (addrString != null)
                return addrString.text;
            else return null;
        }
    }

    public string Port
    {
        get
        {
            if (portString != null)
                return portString.text;
            else return null;
        }
    }

    void Awake()
    {
        //Goes Panel -> JoinContainer -> (1: addr, 2:port) -> Text component
        addrString = transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>();
        portString = transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<Text>();
        hostButton = transform.GetChild(1).GetComponent<Button>(); //Get buttons for method delegation
        joinButton = transform.GetChild(2).GetChild(0).GetComponent<Button>();

        CustomNetworkManager manager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();

        hostButton.onClick.AddListener(delegate { manager.StartHost_Button(); });
        joinButton.onClick.AddListener(delegate { manager.StartClient_Button(); });

    }

    


}
