using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public class CustomNetworkManager : NetworkManager  
{
    public List<GameObject> fishList;
    private NetworkStartPosition[] spawnPoints;
    private int spawnNum;

    public override void OnStartServer() {
        fishList = new List<GameObject>(GunfishList.Get());
        spawnNum = 0;

        //pointTable = new Dictionary<NetworkConnection, int>();
    }

    public override void OnClientConnect (NetworkConnection conn) {
        base.OnClientConnect (conn);

        //pointTable.Add(conn, 0);
        //print("I AM HERE");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        //base.OnServerAddPlayer(conn, playerControllerId);
        spawnPoints = FindObjectsOfType<NetworkStartPosition>(); //Get list of all spawn points in the scene

        //If there aren't any spawn points in the scene, spawn players at the origin
        Vector3 targetPosition = (spawnPoints.Length > 0 ? spawnPoints[(spawnNum) % spawnPoints.Length].transform.position : Vector3.zero);

        //Assign the players a random fish when they join
        //TODO: Replace random with fish selection
        GameObject player = (GameObject)Instantiate(fishList[Random.Range(0,fishList.Count)], targetPosition, Quaternion.identity);
        string playerName = "Player " + (conn.connectionId + 1);
        if (RaceManager.instance && RaceManager.instance.pointTable.ContainsKey(conn)) {
            if (RaceManager.instance.pointTable[conn] > 0) {
                playerName += "\nPoints: " + RaceManager.instance.pointTable[conn];
            }
        } else {
            //print("Nope!");
        }

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        StartCoroutine(SetRpc(player, playerName));

        ConnectionManager.instance.AddGunfish(player.GetComponent<Gunfish>());

        spawnNum++;
    }

    public IEnumerator SetRpc (GameObject player, string playerName) {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        player.GetComponent<Gunfish>().RpcSetName(playerName);
    }

    public override void OnServerRemovePlayer (NetworkConnection conn, UnityEngine.Networking.PlayerController player) {
        //base.OnServerRemovePlayer(conn, player);
        ConnectionManager.instance.RemoveGunfish(conn);
        //print("Removing player");
        RaceManager.instance.TrySwapLevel();
    }

    //IEnumerator TriggerStartEvent () {
    //    int seconds = 10;

    //    while (seconds > 0) {
    //        print(seconds);
    //        yield return new WaitForSeconds(1f);
    //        seconds--;
    //    }

    //    EventManager.TriggerEvent(EventType.NextLevel);
    //}

    public override void OnServerSceneChanged (string sceneName) {
        base.OnServerSceneChanged (sceneName);
    }





    //UI
    /***************************************************/

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