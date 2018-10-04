using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {
    //A temporary override for Network Manager to expose some important features that UI doesn't normally have access to.



    /// <summary>
    /// A wrapper for NetworkManager's StartHost method. 
    /// </summary>
    public void StartHost_Button()
    {
        base.StartHost();
    }

    public void StartClient_Button()
    {
        base.StartClient();
    }

    public void UpdateAddress()
    {
        networkAddress = FindMainMenu().GetComponent<MainMenuManager>().Addr;
    }

    public void UpdatePort()
    {
        string port = FindMainMenu().GetComponent<MainMenuManager>().Port;
        int res;
        bool success = int.TryParse(port, out res);
        if(success)
            networkPort = res;
        else
        {
            Debug.Log("Failed to set port");
        }
    }

    public GameObject FindMainMenu()
    {
        return GameObject.Find("mainmenu");
    }

}
