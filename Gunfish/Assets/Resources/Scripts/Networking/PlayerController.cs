﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public GameObject gunshotDebris;
    public AudioClip[] gunshotAudio;

    public static Gunfish ownedGunfish;

    private void Start () {
        if (gunshotDebris == null) {
            gunshotDebris = Resources.Load<GameObject>("Prefabs/Debris");
        }

        gunshotAudio = Resources.LoadAll<AudioClip>("Audio/Shots/");

        //Debug.Log("ConnId: " + connectionToServer.connectionId);

        NetworkManager.singleton.client.RegisterHandler(MessageTypes.DEBUGLOGMSG, OnDebugLog);

        NetworkManager.singleton.client.RegisterHandler(MessageTypes.RAYHIT, OnRayHit);
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.MULTIRAYHIT, OnMultiRayHit);
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.GUNSHOT, OnGunshot);
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
        if (!rayHitInfo.netId.IsEmpty()) {
            //EffectsManager.DisplayBulletHit(rayHitInfo.end, rayHitInfo.normal, rayHitInfo.color);

            if (rayHitInfo.netId == ownedGunfish.netId) {
                ownedGunfish.Hit(-rayHitInfo.normal.normalized, rayHitInfo.end);
            }
        }

        //EffectsManager.DisplayBulletLine(rayHitInfo.origin, rayHitInfo.end);
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

    /*
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
        */

    #endregion
}