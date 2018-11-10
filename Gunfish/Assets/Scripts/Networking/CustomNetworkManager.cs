using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public class CustomNetworkManager : NetworkManager  
{
    public List<GameObject> fishList;
    private NetworkStartPosition[] spawnPoints;
    private int spawnNum;

    public static Dictionary<NetworkConnection, int> pointTable;

    public override void OnStartServer() {
        fishList = new List<GameObject>(GunfishList.Get());
        spawnNum = 0;

        pointTable = new Dictionary<NetworkConnection, int>();
    }

    public override void OnClientConnect (NetworkConnection conn) {
        base.OnClientConnect (conn);

        pointTable.Add(conn, 0);
        //print("I AM HERE");
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        //base.OnServerAddPlayer(conn, playerControllerId);
        spawnPoints = FindObjectsOfType<NetworkStartPosition>(); //Get list of all spawn points in the scene

        //If there aren't any spawn points in the scene, spawn players at the origin
        Vector3 targetPosition = (spawnPoints.Length > 0 ? spawnPoints[(spawnNum++) % spawnPoints.Length].transform.position : Vector3.zero);

        //Assign the players a random fish when they join
        //TODO: Replace random with fish selection
        GameObject player = (GameObject)Instantiate(fishList[Random.Range(0,fishList.Count)], targetPosition, Quaternion.identity);
        
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        //print("Adding player");

        ConnectionManager.instance.AddGunfish(player.GetComponent<Gunfish>());
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
}