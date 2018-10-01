using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public GameObject gunshotDebris;
    public AudioClip[] gunshotAudio;

    //public override void OnStartLocalPlayer () {
    //    NetworkManager.singleton.client.RegisterHandler(MessageTypes.DEBUGLOGMSG, OnDebugLog);
    //    NetworkManager.singleton.client.RegisterHandler(MessageTypes.GUNSHOTPARTICLEMSG, OnGunshotParticle);

    //}

    private void Start () {
        if (gunshotDebris == null) {
            gunshotDebris = Resources.Load<GameObject>("Prefabs/Debris");
        }

        gunshotAudio = Resources.LoadAll<AudioClip>("Audio/Shots/");

        //Debug.Log("ConnId: " + connectionToServer.connectionId);

        NetworkManager.singleton.client.RegisterHandler(MessageTypes.DEBUGLOGMSG, OnDebugLog);
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.GUNSHOTPARTICLEMSG, OnGunshotParticle);
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.GUNSHOTAUDIOMSG, OnGunshotAudio);
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.GUNSHOT, OnGunshot);
    }

    #region MESSAGE HANDLERS

    private void OnGunshotAudio (NetworkMessage netMsg) {
        //Debug.Log("Client is good");
        GunshotAudioMsg msg = netMsg.ReadMessage<GunshotAudioMsg>();

        GameObject audioObj = new GameObject();
        audioObj.name = "GunshotAudioSource";
        audioObj.transform.position = msg.position;
        AudioSource audioSource = audioObj.AddComponent<AudioSource>();
        audioSource.clip = gunshotAudio[msg.clipIndex];
        audioSource.Play();
        Destroy(audioObj, audioSource.clip.length);
    }

    private void OnGunshotParticle (NetworkMessage netMsg) {
        GunshotParticleMsg msg = netMsg.ReadMessage<GunshotParticleMsg>();

        Debug.Log("AHHHH");

        GameObject hitDebris = Instantiate<GameObject>(
            gunshotDebris, 
            msg.position, 
            Quaternion.Euler(msg.normal)
        );

        var main = hitDebris.GetComponent<ParticleSystem>().main;
        main.startColor = new Color(msg.r, msg.g, msg.b);
        Destroy(hitDebris, 2f);
    }

    private void OnDebugLog (NetworkMessage netMsg) {
        DebugLogMsg msg = netMsg.ReadMessage<DebugLogMsg>();

        Debug.Log(msg.log);
    }

    private void OnGunshot (NetworkMessage netMsg)
    {
        GunshotMsg msg = netMsg.ReadMessage<GunshotMsg>();
        Gunfish gunfish = ClientScene.FindLocalObject(msg.netId).GetComponent<Gunfish>();
        gunfish.DisplayShoot();
    }

    #endregion
}




//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//public class PlayerController : NetworkBehaviour {

//    [Header("Input")]
//    public int moveX;
//    private byte compressedMoveX;
//    public bool shooting;

//    [Header("Player Info")]
//    public GameObject localPlayerConnectionObj;
//    public GameObject gunfish;

//    public override void OnStartClient () {
        
//    }

//    public override void OnStartLocalPlayer () {
//        NetworkManager.singleton.client.RegisterHandler(MessageTypes.DEBUGLOGMSG, OnDebugLog);
//        NetworkManager.singleton.client.RegisterHandler(MessageTypes.GUNFISHMSG, OnGunfish);
//        NetworkManager.singleton.client.RegisterHandler(MessageTypes.SPAWNMSG, OnSpawnGameObject);
//        Debug.Log("ConnId: " + connectionToServer.connectionId);
//    }

//    #region MESSAGE HANDLERS

//    private void OnSpawnGameObject (NetworkMessage netMsg) {
//        NetIdMsg msg = netMsg.ReadMessage<NetIdMsg>();


//    }

//    private void OnDebugLog (NetworkMessage netMsg) {
//        DebugLogMsg msg = netMsg.ReadMessage<DebugLogMsg>();

//        Debug.Log(msg.log);
//    }

//    private void OnGunfish (NetworkMessage netMsg) {
//        GunfishMsg msg = netMsg.ReadMessage<GunfishMsg>();

//        Transform fish = NetworkServer.FindLocalObject(msg.netId).transform;

//        byte childCount = (byte)fish.childCount;

//        fish.position = msg.startPosition;
//        fish.eulerAngles = new Vector3(0f, 0f, msg.startRotation);
//        fish.localScale = msg.startScale;
//        fish.GetComponent<Rigidbody2D>().velocity = msg.startVelocity;

//        for (int i = 0; i < childCount; i++) {
//            Transform child = fish.GetChild(i);

//            child.position = msg.positions[i];
//            child.eulerAngles = new Vector3(0f, 0f, msg.rotations[i]);
//            child.localScale = msg.scales[i];
//            child.GetComponent<Rigidbody2D>().velocity = msg.velocities[i];
//        }
//    }

//    #endregion
//}
