using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnection : NetworkBehaviour {

	[SerializeField] private GameObject gunfishPrefab;
    private string fishPath = "Prefabs/Gunfish/Squadfish";

    public int score;
    public int place;

	//// Use this for initialization
	override public void OnStartLocalPlayer () {
        
        NetworkManager.singleton.client.Send(MessageTypes.SPAWNMSG, new SpawnMsg(fishPath, transform.position, gameObject));
    }

	//[Command] private void CmdSpawnFish () {
	//	GameObject go = Instantiate (gunfishPrefab, transform.position, Quaternion.identity) as GameObject;
 //       NetworkServer.SpawnWithClientAuthority (go, connectionToClient);
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}