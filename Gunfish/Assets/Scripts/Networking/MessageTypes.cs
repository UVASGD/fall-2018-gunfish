using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MessageTypes {
    //Debug/Misc (100-199)
    public static short DEBUGLOGMSG = 100;
    public static short INPUTMSG = 101;

    //GameObject (200-299)
    //public static short NETIDMSG = 200;
    public static short SPAWNMSG = 200;

    //Gun Messages
    public static short RAYHIT = 300;
    public static short MULTIRAYHIT = 301;
    public static short GUNSHOT = 302;

    //Race Messages
    public static short PLAYERFINISH = 400;
}
