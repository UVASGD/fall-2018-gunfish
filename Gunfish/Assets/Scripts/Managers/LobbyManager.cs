using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyManager : NetworkLobbyManager
{

    public static LobbyManager instance;

    private string playerName;
    private GameObject[] gunfishList;
    private int gunfishIndex;

    public int playerCount = 0;
    private int maxPlayerCount = 4;

    // Use this for initialization
    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(instance);
        }

        gunfishList = GunfishList.Get();
        gunfishIndex = 0;
        //NetworkLobbyManager.singleton.OnServerConnect(
    }

    public void StartGame()
    {


    }
    //public override 

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        //base.OnServerConnect(conn);

        Debug.Log("Whatcha doing fam");
    }
}