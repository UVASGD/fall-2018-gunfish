using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public GameObject gunshotDebris;
    public AudioClip[] gunshotAudio;

    public static Gunfish ownedGunfish;

    private void Start () {
        DontDestroyOnLoad(gameObject);
        if (gunshotDebris == null) {
            gunshotDebris = Resources.Load<GameObject>("Prefabs/Debris");
        }

        gunshotAudio = Resources.LoadAll<AudioClip>("Audio/Shots/");

        //Debug.Log("ConnId: " + connectionToServer.connectionId);

        NetworkManager.singleton.client.RegisterHandler(MessageTypes.DEBUGLOGMSG, OnDebugLog);

        NetworkManager.singleton.client.RegisterHandler(MessageTypes.RAYHIT, OnRayHit);
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.MULTIRAYHIT, OnMultiRayHit);
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.GUNSHOT, OnGunshot);

        foreach (Gunfish fish in FindObjectsOfType<Gunfish>()) {
            if (fish.hasAuthority) {
                ownedGunfish = fish;
            }
        }
    }

    #region MESSAGE HANDLERS

    private void OnRayHit(NetworkMessage netMsg) {
        RayHitMsg msg = netMsg.ReadMessage<RayHitMsg>();

        HandleHitInfo(msg.rayHitInfo);
    }

    private void OnMultiRayHit (NetworkMessage netMsg) {
        MultiRayHitMsg msg = netMsg.ReadMessage<MultiRayHitMsg>();

        foreach (RayHitInfo rayHitInfo in msg.rayHitInfos) {
            HandleHitInfo(rayHitInfo);
        }
    }

    private void HandleHitInfo(RayHitInfo rayHitInfo) {
        if (rayHitInfo.netId != NetworkInstanceId.Invalid) {
            EffectsManager.instance.DisplayBulletHit(rayHitInfo.end, rayHitInfo.normal, rayHitInfo.color);

            if (rayHitInfo.netId == ownedGunfish.netId) {
                ownedGunfish.Hit((rayHitInfo.end - rayHitInfo.origin).normalized);
            }
        }

        EffectsManager.instance.DisplayBulletLine(rayHitInfo.origin, rayHitInfo.end);
    }

    private void OnGunshot(NetworkMessage netMsg) {
        GunfishMsg msg = netMsg.ReadMessage<GunfishMsg>();
        Gunfish gunfish = ClientScene.FindLocalObject(msg.netId).GetComponent<Gunfish>();
        gunfish.DisplayShoot();
    }

    private void OnDebugLog(NetworkMessage netMsg) {
        DebugLogMsg msg = netMsg.ReadMessage<DebugLogMsg>();

        Debug.Log(msg.log);
    }
    #endregion
}
