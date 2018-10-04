//Gun.cs
//Written by Ryan Kann
//
//Purpose: 
//To store important variables pertaining to a gun GameObject
//
//How to Use: 
//Set the variables to whatever you want the particular gun
//to be. There is a Gun creation Editor Window that can help punch these
//in automatically, you should use the Inspector window to edit an already
//existing gun.
//These variables are only referenced/used in Gunfish.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
    public float force = 3000f;
    public float maxFireCD = 1f;
    public GameObject fireFX;

    //Gunshot audio and visual fx
    public void DisplayShoot()
    {
        fireFX.GetComponent<AudioSource>().Play();
        fireFX.GetComponent<ParticleSystem>().Play();
    }
}
