using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MessageTypes {
    //Debug/Misc (100-199)
    public static short DEBUGLOGMSG = 100;
    public static short INPUTMSG = 101;

    //GameObject (200-299)
    public static short NETIDMSG = 200;
    public static short SPAWNMSG = 201;

    //Gunfish Specific (300-399)
    public static short GUNFISHMSG = 300;
    public static short GUNSHOTHITMSG = 301;
    public static short GUNSHOTPARTICLEMSG = 302;
    public static short GUNSHOTAUDIOMSG = 303;

    //Gun Messages
    public static short RAYHIT = 304;
    public static short MULTIRAYHIT = 305;
    public static short GUNSHOT = 306;
}
