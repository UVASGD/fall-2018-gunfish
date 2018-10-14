using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    private Text addrString;
    private Text portString;

    private bool settingsOpen = false;

    Button joinButton;
    Button hostButton;
    InputField addrField;
    InputField portField;

    Transform mainMenu;
    Transform settingsMenu;

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
        mainMenu = transform.GetChild(0);
        settingsMenu = transform.GetChild(1);

        //Goes Panel -> JoinContainer -> (1: addr, 2:port) -> Text component
        addrField = mainMenu.GetChild(2).GetChild(1).GetComponent<InputField>();
        portField = mainMenu.GetChild(2).GetChild(2).GetComponent<InputField>();
        hostButton = mainMenu.GetChild(1).GetComponent<Button>(); //Get buttons for method delegation
        joinButton = mainMenu.GetChild(2).GetChild(0).GetComponent<Button>();
        addrString = mainMenu.GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>();
        portString = mainMenu.GetChild(2).GetChild(2).GetChild(1).GetComponent<Text>();

        CustomNetworkManager manager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();

        hostButton.onClick.AddListener(delegate { manager.StartHost_Button(); });
        joinButton.onClick.AddListener(delegate { manager.StartClient_Button(); });
        addrField.onEndEdit.AddListener(delegate { manager.UpdateAddress(); });
        portField.onEndEdit.AddListener(delegate { manager.UpdatePort(); });

    }

    /// <summary>
    /// Call to quit the game.
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("We'd quit if we weren't in the editor right now...");
        Application.Quit();
    }

    public void ToggleSettings()
    {
        if(settingsOpen)
        {
            settingsMenu.gameObject.SetActive(false);
            mainMenu.gameObject.SetActive(true);
            settingsOpen = false;
        }
        else
        {
            settingsMenu.gameObject.SetActive(true);
            mainMenu.gameObject.SetActive(false);
            settingsOpen = true;
        }
    }

    


}
