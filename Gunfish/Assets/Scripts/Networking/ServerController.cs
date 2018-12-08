using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerController : NetworkBehaviour {

    public override void OnStartServer () {
        ConnectionConfig config = new ConnectionConfig();
        config.DisconnectTimeout = 5000; //If the player times out for 5 seconds, disconnect them

        config.NetworkDropThreshold = 45;
        config.OverflowDropThreshold = 45;
        config.AckDelay = 200;
        config.AcksType = ConnectionAcksType.Acks128;
        config.MaxSentMessageQueueSize = 300;

        NetworkServer.RegisterHandler(MessageTypes.GUNSHOT, OnGunshot);
        NetworkServer.RegisterHandler(MessageTypes.CHANGEFEEESH, OnChangeFeesh);
    }

    public void SendClientDebugLog (string msg) {
        NetworkServer.SendToAll(MessageTypes.DEBUGLOGMSG, new DebugLogMsg(msg));
    }

    #region MESSAGE HANDLERS

    public void OnGunshot(NetworkMessage netMsg)
    {
        GunfishMsg msg = netMsg.ReadMessage<GunfishMsg>();

        //Get the gunfish, tell it to shoot, and get the hit information
        Gunfish gunfish = NetworkServer.FindLocalObject(msg.netId).GetComponent<Gunfish>();

        //Presuming our gun is a single raycaster
        RayHitInfo rayHitInfo = gunfish.ServerShoot(gunfish);
        NetworkServer.SendToAll(MessageTypes.RAYHIT, new RayHitMsg(rayHitInfo));

        //This message handles gunshot audio and muzzle flash
        NetworkServer.SendToAll(MessageTypes.GUNSHOT, new GunfishMsg(msg.netId));
    }

    public void OnChangeFeesh(NetworkMessage netMsg) {
        GunfishSelectMsg msg = netMsg.ReadMessage<GunfishSelectMsg>();

        Gunfish gunfish = NetworkServer.FindLocalObject(msg.netId).GetComponent<Gunfish>();

        //GunfishSelectMsg gunMsg = new GunfishSelectMsg(msg.netId, gunfish.ChangeFeesh());

        //NetworkServer.SendToAll(MessageTypes.CHANGEFEEESH, msg);

        gunfish.ChangeFeesh(msg.index);
    }
    #endregion
}