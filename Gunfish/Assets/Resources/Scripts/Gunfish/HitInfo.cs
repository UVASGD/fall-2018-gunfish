using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct RayHitInfo {
    public NetworkInstanceId netId;
    public Vector2 origin;
    public Vector2 end;
    public Vector2 normal;
    public Color color;

    /*
    public RayHitInfo(NetworkInstanceId netId, Vector2 normal, Vector2 origin, Vector2 end, Color color) {
        this.netId = netId;
        this.normal = normal;
        this.origin = origin;
        this.end = end;
        this.color = color;
    }
    //hitId
    //normal
    //origin
    //end
    //color
    //GunEnum
    */
}
