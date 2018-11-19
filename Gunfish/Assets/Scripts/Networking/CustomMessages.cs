using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Debug/Misc
/*****************************************/
public class DebugLogMsg : MessageBase {
    public string log;

    public DebugLogMsg() { }

    public DebugLogMsg(string _log) {
        log = _log;
    }
}

//GameObject
/*****************************************/
public class GunfishMsg : MessageBase {
    public NetworkInstanceId netId;

    public GunfishMsg() { }

    public GunfishMsg(NetworkInstanceId id) {
        netId = id;
    }
}

public class SpawnMsg : MessageBase
{
    public string path;
    public Vector2 pos;
    public GameObject player;
    public SpawnMsg()
    {
    }
    public SpawnMsg(string path, Vector2 pos,GameObject player)
    {
        this.path = path;
        this.pos = pos;
        this.player = player;
    }
}


//Gun
/*****************************************/
public class RayHitMsg : MessageBase {
    public RayHitInfo rayHitInfo;

    public RayHitMsg() { }

    public RayHitMsg(RayHitInfo hitInfo) {
        this.rayHitInfo = hitInfo;
    }
}

public class MultiRayHitMsg : MessageBase {
    public RayHitInfo[] rayHitInfos;

    public MultiRayHitMsg() { }

    public MultiRayHitMsg(RayHitInfo[] hitInfos) {
        this.rayHitInfos = hitInfos;
    }
}


//Race
/*****************************************/
public class PlayerFinishMsg : MessageBase {
    public GameObject gunfish;

    public PlayerFinishMsg() { }

    public PlayerFinishMsg(GameObject fish) {
        gunfish = fish;
    }
}

public class RequestTimeMsg : MessageBase {
    public int time;

    public RequestTimeMsg() { }

    public RequestTimeMsg(int time) {
        this.time = time;
    }
}