using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public class CustomNetworkManager : NetworkManager  
{
    public List<GameObject> fishList;
    private NetworkStartPosition[] spawnPoints;
    public List<NetworkConnection> networkConnections;
    public List<NetworkInstanceId> netIds;

    public override void OnStartServer()
    {
        fishList = new List<GameObject>(GunfishList.Get());
    }
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        Debug.Log("SpawnPoints count: " + spawnPoints.Length);
        GameObject player = (GameObject)Instantiate(fishList[Random.Range(0,fishList.Count)], spawnPoints[0].transform.position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        networkConnections.Add(conn);
        netIds.Add(player.GetComponent<Gunfish>().netId);
    }

    public override void OnServerRemovePlayer (NetworkConnection conn, UnityEngine.Networking.PlayerController player) {
        base.OnServerRemovePlayer (conn, player);
        networkConnections.Remove(conn);

        foreach (NetworkInstanceId fish in conn.clientOwnedObjects) {
            netIds.Remove(fish);
        }
    }
}