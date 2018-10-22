using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public class CustomNetworkManager : NetworkManager  
{
    public List<GameObject> fishList;
    private NetworkStartPosition[] spawnPoints;
    //public List<NetworkConnection> networkConnections;
    //public List<NetworkInstanceId> netIds;

    public override void OnStartServer()
    {
        fishList = new List<GameObject>(GunfishList.Get());
    }
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        spawnPoints = FindObjectsOfType<NetworkStartPosition>(); //Get list of all spawn points in the scene

        //If there aren't any spawn points in the scene, spawn players at the origin
        Vector3 targetPosition = (spawnPoints.Length > 0 ? spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position : Vector3.zero);

        //Assign the players a random fish when they join
        //TODO: Replace random with fish selection
        GameObject player = (GameObject)Instantiate(fishList[Random.Range(0,fishList.Count)], targetPosition, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        //networkConnections.Add(conn);
        //netIds.Add(player.GetComponent<Gunfish>().netId);
        //ConnectionManager.instance.netConns.Add(conn);
        //ConnectionManager.instance.gunfish.Add(player.GetComponent<Gunfish>());
    }

    public override void OnServerRemovePlayer (NetworkConnection conn, UnityEngine.Networking.PlayerController player) {
        base.OnServerRemovePlayer (conn, player);
        //ConnectionManager.instance.netConns.Remove(conn);

        //foreach (NetworkInstanceId fish in conn.clientOwnedObjects) {
        //    ConnectionManager.instance.gunfish.Remove(NetworkServer.FindLocalObject(fish).GetComponent<Gunfish>());
        //}
    }
}