using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class DebugLogMsg : MessageBase {
    public string log;

    public DebugLogMsg() { }

    public DebugLogMsg(string _log) {
        log = _log;
    }
}

public class NetIdMsg : MessageBase { 
    public NetworkInstanceId netId;

    public NetIdMsg () { }

    public NetIdMsg (NetworkInstanceId netId) {
        this.netId = netId;
    }
}

public class GunshotHitMsg : MessageBase {
    public Vector2 force;
    public float damage;
    public Vector3 position;

    public GunshotHitMsg () { }

    public GunshotHitMsg (Vector2 force) {
        this.force = force;
        damage = 0f;
        this.position = Vector3.zero;
    }

    public GunshotHitMsg (Vector2 force, float damage) {
        this.force = force;
        this.damage = damage;
        this.position = Vector3.zero;
    }

    public GunshotHitMsg (Vector2 force, float damage, Vector3 position) {
        this.force = force;
        this.damage = damage;
        this.position = position;
    }
}

public class GunshotParticleMsg : MessageBase {
    public Vector3 origin;
    public Vector3 position;
    public Vector2 normal;

    //Color components
    public float r;
    public float g;
    public float b;
    public float a;

    public GunshotParticleMsg () { }

    public GunshotParticleMsg (Vector3 position) {
        this.position = position;
        this.origin = Vector3.zero;
        this.normal = Vector2.zero;
        this.r = 0f;
        this.g = 0f;
        this.b = 0f;
        this.a = 1f;
    }

    public GunshotParticleMsg (Vector3 position, Vector3 origin) {
        this.position = position;
        this.origin = origin;
        this.normal = Vector2.zero;
        this.r = 0f;
        this.g = 0f;
        this.b = 0f;
        this.a = 1f;
    }

    public GunshotParticleMsg (Vector3 position, Vector2 normal) {
        this.position = position;
        this.origin = Vector3.zero;
        this.normal = normal;
        this.r = 0f;
        this.g = 0f;
        this.b = 0f;
        this.a = 1f;
    }

    public GunshotParticleMsg (Vector3 position, Vector3 origin, Vector2 normal) {
        this.position = position;
        this.origin = this.normal;
        this.normal = normal;
        this.r = 0f;
        this.g = 0f;
        this.b = 0f;
        this.a = 1f;
    }

    public GunshotParticleMsg (Vector3 position, Vector2 normal, float r, float g, float b) {
        this.position = position;
        this.origin = Vector3.zero;
        this.normal = normal;
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = 1f;
    }

    public GunshotParticleMsg (Vector3 position, Vector3 origin, Vector2 normal, float r, float g, float b) {
        this.position = position;
        this.origin = origin;
        this.normal = normal;
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = 1f;
    }

    public GunshotParticleMsg (Vector3 position, Vector2 normal, float r, float g, float b, float a) {
        this.position = position;
        this.origin = Vector3.zero;
        this.normal = normal;
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public GunshotParticleMsg (Vector3 position, Vector3 origin, Vector2 normal, float r, float g, float b, float a) {
        this.position = position;
        this.origin = origin;
        this.normal = normal;
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
}

public class GunshotAudioMsg : MessageBase {
    public short clipIndex;
    public Vector3 position;

    public GunshotAudioMsg () { }

    public GunshotAudioMsg (short clipIndex) {
        this.clipIndex = clipIndex;
        this.position = Vector3.zero;
    }

    public GunshotAudioMsg (short clipIndex, Vector3 position) {
        this.clipIndex = clipIndex;
        this.position = position;
    }
}

public class RayHitMsg : MessageBase {
    public RayHitInfo rayHitInfo;

    public RayHitMsg() { }

    public RayHitMsg(RayHitInfo rayHitInfo) {
        this.rayHitInfo = rayHitInfo;
    }
}

public class MultiRayHitMsg : MessageBase {
    public RayHitInfo[] rayHitInfo;

    public MultiRayHitMsg() { }

    public MultiRayHitMsg( RayHitInfo[] rayHitInfo, Vector2 origin) {
        this.rayHitInfo = rayHitInfo;
    }
}

public struct RayHitInfo {
    public NetworkInstanceId netId;

    public Vector2 origin;
    public Vector2 end;

    public Vector2 normal;
    public Color color;

    public RayHitInfo(NetworkInstanceId netId, Vector2 origin, Vector2 end, Vector2 normal, Color color) {
        this.netId = netId;
        this.origin = origin;
        this.end = end;
        this.normal = normal;
        this.color = color;
    }
}

public class GameObjectMsg : MessageBase { 
    public GameObject obj;

    public GameObjectMsg () { }

    public GameObjectMsg (GameObject obj) {
        this.obj = obj;
    }
}

public class InputMsg : MessageBase {
    //0 = not moving, 1 = left, 2 = right;
    public byte movement;
    public bool shoot;

    public GameObject fish;

    public InputMsg() { }

    public InputMsg(byte move, bool fire, GameObject gunfish) {
        movement = move;
        shoot = fire;

        fish = gunfish;
    }
}