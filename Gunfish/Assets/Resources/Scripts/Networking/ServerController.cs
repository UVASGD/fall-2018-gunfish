using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerController : NetworkBehaviour {

    public override void OnStartServer () {
        ConnectionConfig config = new ConnectionConfig();
        config.DisconnectTimeout = 5000; //If the player times out for 5 seconds, disconnect them
        Debug.Log("Server start");
        NetworkServer.RegisterHandler(MessageTypes.NETIDMSG, OnNetID);
        NetworkServer.RegisterHandler(MessageTypes.GUNSHOTAUDIOMSG, OnGunshotAudio);
        NetworkServer.RegisterHandler(MessageTypes.SPAWNMSG, OnSpawn);
    }

    public void SendClientDebugLog (string msg) {
        NetworkServer.SendToAll(MessageTypes.DEBUGLOGMSG, new DebugLogMsg(msg));
    }

    #region MESSAGE HANDLERS

    public void OnGunshotAudio (NetworkMessage netMsg) {
        //Debug.Log("Server is good");
        GunshotAudioMsg msg = netMsg.ReadMessage<GunshotAudioMsg>();

        NetworkServer.SendToAll(MessageTypes.GUNSHOTAUDIOMSG, new GunshotAudioMsg(msg.clipIndex, msg.position));
    }

    //This is called when a fish shoots
    public void OnNetID (NetworkMessage netMsg) {
        //Debug.Log("OnNetID called");
        NetIdMsg msg = netMsg.ReadMessage<NetIdMsg>();
        Transform fish = NetworkServer.FindLocalObject(msg.netId).transform;

        //Shoot bullet raycast. Do multiple hits to avoid collision with own fish pieces
        Vector3 direction = fish.GetChild(fish.childCount-1).right.normalized;
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            fish.GetChild(fish.childCount-1).position, direction
        );

        if (hits.Length > 0) {
            bool targetHit = false;
            RaycastHit2D realHit = new RaycastHit2D();
            Color hitColor = Color.red;

            foreach (RaycastHit2D hit in hits) {
                if (hit.collider.CompareTag("Player")) {
                    if (hit.transform == fish) continue; //ignore collisions with self
                    else if (hit.transform.parent != null && hit.transform.parent == fish) continue;

                    Debug.Log("Hit: " + hit.collider.name);

                    realHit = hit;
                    targetHit = true;

                    //If it's a fish, ensure it's applied to the root
                    GameObject target = null;
                    if (hit.transform.parent == null) {
                        target = hit.transform.gameObject;
                    } else if (hit.transform.parent.CompareTag("Player")) {
                        target = hit.transform.parent.gameObject;
                    }

                    if (target != null) {
                        NetworkServer.SendToClientOfPlayer(target,
                                                   MessageTypes.GUNSHOTHITMSG,
                                                   new GunshotHitMsg(direction * 200f, 1f, realHit.point)
                                                  );
                    }
                    break;
                } else if (hit.collider.CompareTag("Ground")) {
                    realHit = hit;
                    targetHit = true;
                    hitColor = hit.collider.GetComponent<SpriteRenderer>().color;
                    break;
                }
            }

            if (targetHit) {
                NetworkServer.SendToAll(MessageTypes.GUNSHOTPARTICLEMSG, 
                                        new GunshotParticleMsg(realHit.point,
                                                               fish.position,
                                                               realHit.normal,
                                                               hitColor.r,
                                                               hitColor.g,
                                                               hitColor.b
                                                              )
                                       );
            }
        }
    }
    private void OnSpawn(NetworkMessage netMsg)
    {

        SpawnMsg msg = netMsg.ReadMessage<SpawnMsg>();
        GameObject fish = Instantiate(Resources.Load(msg.path), msg.pos, Quaternion.identity)as GameObject;
        NetworkServer.SpawnWithClientAuthority(fish,msg.player);
    }

    #endregion
}
