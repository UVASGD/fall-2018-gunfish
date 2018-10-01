using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerConnection : NetworkBehaviour
{
    public string fishPath = "Prefabs/GunFish/SquadFish";
    public GameObject gunFish;
    public GameObject gun;
	// Use this for initialization
	void Start ()
    {
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.SPAWNMSG, );

    }
    public override void OnStartLocalPlayer()
    {
    
        NetworkManager.singleton.client.Send(MessageTypes.SPAWNMSG, new SpawnMsg(fishPath,transform.position,gameObject));
    }
    // Update is called once per frame
    void Update ()
    {
		
	}
}
