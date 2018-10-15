using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct HitInfo {
    NetworkInstanceId hitId;
    Vector2 normal;
    Vector2 origin;
    Vector2 end;
    Color color;

    public HitInfo(NetworkInstanceId hitId, Vector2 normal, Vector2 origin, Vector2 end, Color color) {
        this.hitId = hitId;
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
}
