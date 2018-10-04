using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    private Text addrString;
    private Text portString;

    Button joinButton;
    Button hostButton;
    InputField addrField;
    InputField portField;

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
        addrField = transform.GetChild(2).GetChild(1).GetComponent<InputField>();
        portField = transform.GetChild(2).GetChild(2).GetComponent<InputField>();
        hostButton = transform.GetChild(1).GetComponent<Button>(); //Get buttons for method delegation
        joinButton = transform.GetChild(2).GetChild(0).GetComponent<Button>();
        addrString = transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>();
        portString = transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<Text>();

        CustomNetworkManager manager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();

        hostButton.onClick.AddListener(delegate { manager.StartHost_Button(); });
        joinButton.onClick.AddListener(delegate { manager.StartClient_Button(); });
        addrField.onEndEdit.AddListener(delegate { manager.UpdateAddress(); });
        portField.onEndEdit.AddListener(delegate { manager.UpdatePort(); });

    }

    


}
