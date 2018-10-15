using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour {


	private List<NetworkConnection> gunFishClient = new List<NetworkConnection>();
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnServerConnect(NetworkConnection client){
		if (client.hostID >= 0){
			Debug.Log("New Player has Joined - " + client.connectionId);
			gunFishClient.Add(client);
		}
	}

	public override void OnServerDisconnect(NetworkConnection client){
		for(int i = gunFishClient.size(); i >=0; i--){
			if(gunFishClient.get(i).connectionId.equals(client.connectionId)){
				gunFishClient.RemoveAt(i);
				break;
			}
		}
	}
}
