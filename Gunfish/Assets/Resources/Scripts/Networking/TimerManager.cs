using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TimerManager : NetworkBehaviour {

    [SyncVar]
    float gameTime; //Game length

    public float CurTime
    {
        get { return gameTime; }
    }

    [SyncVar]
    int state = 0; //0 = Waiting for Ready. 1 = Game running. 2 = Game is done

    [SyncVar]
    int playerCount; //How many players are currently in the game.

    static TimerManager server;
    public static TimerManager ServerTimer
    {
        get { return server; }
    }

    void Start()
    {
        if(isServer)
        {
            if(isLocalPlayer) //We are the server
            {
                server = this;
            }
        }
    }

    void Update()
    {
        if(isServer) //Server time update
        {
            switch (state)
            {
                case 0: //Waiting on players to be ready
                    if (false) //This should check if everyone is ready
                    {
                        state = 1;
                        gameTime = 0;
                    }
                    break;

                case 1: //Running as normal
                    gameTime += Time.deltaTime;
                    break;

                case 2: //Game end
                    //Level has finished
                    break;
            }
            
        }
   
    }



}
