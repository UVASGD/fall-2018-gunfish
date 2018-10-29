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
    public HitType hitType;
}
