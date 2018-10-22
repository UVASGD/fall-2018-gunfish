using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public class CustomNetworkManager : NetworkManager  
{
    public List<GameObject> fishList;
    private NetworkStartPosition[] spawnPoints;

    public override void OnStartServer()
    {
        fishList = new List<GameObject>(GunfishList.Get());
    }
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        Debug.Log("SpawnPoints count: " + spawnPoints.Length);
        GameObject player = (GameObject)Instantiate(fishList[Random.Range(0,fishList.Count)], spawnPoints[0].transform.position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}