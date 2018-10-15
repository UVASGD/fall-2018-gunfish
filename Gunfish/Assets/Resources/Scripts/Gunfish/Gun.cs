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
using UnityEngine.Networking;

public class Gun : MonoBehaviour {
    public float force = 3000f;
    public float maxFireCD = 1f;
    public float distance = 50f;
    public GameObject barrelPoint;
    public AudioSource boomSound;
    public ParticleSystem muzzleFlash;

    private void Start() {
        boomSound = GetComponentInChildren<AudioSource>();
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    //We're just treating gun as a single raycaster, but making a multiraycaster should be very easy
    public RayHitInfo ServerShoot() {

        RayHitInfo rayHitInfo = new RayHitInfo();

        RaycastHit2D rayHit = Physics2D.Raycast(barrelPoint.transform.position, transform.right, distance);
        if (rayHit) {
            GameObject hit = rayHit.collider.gameObject;

            //if gunfish
            if (hit.CompareTag("Gunfish")) {
                rayHitInfo.netId = hit.GetComponentInParent<Gunfish>().netId;
                rayHitInfo.color = Color.red;
            }

            //if generic object
            else if (hit.CompareTag("Environment")) {
                //rayHitInfo.netId.Value defaults to zero
                rayHitInfo.color = hit.gameObject.GetComponent<SpriteRenderer>().color;
            }

            rayHitInfo.normal = rayHit.normal;
        }
        else {
            //if nothing was hit
            rayHitInfo.netId = NetworkInstanceId.Invalid;
        }

        rayHitInfo.origin = barrelPoint.transform.position;
        rayHitInfo.end = rayHit.point;

        return rayHitInfo;
    }

    //Gunshot audio and visual fx
    public void DisplayShoot()
    {
        boomSound.Play();
        muzzleFlash.Play();
    }
}
