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

    //We'll want a lot of these variables to reference a scriptableObject
    public ShotInfo shotInfo;
    public Gradient shotTrailColor;
    public GameObject barrelPoint;
    public AudioSource boomSound;
    public LineRenderer muzzleFlash;
    WaitForSeconds flashDuration;
    public GunType gunType = GunType.Ray;
    public ShotType shotType = ShotType.Medium;

    private void Start() {
        boomSound = GetComponentInChildren<AudioSource>();
        muzzleFlash = GetComponentInChildren<LineRenderer>();
    }

    public void SetShotType(ShotType shotType) {
        this.shotType = shotType;
        shotInfo = Misc.ShotDict[shotType];
        flashDuration = new WaitForSeconds(shotInfo.flashDuration);
    }

    //We're just treating gun as a single raycaster, but making a multiraycaster should be very easy
    public RayHitInfo ServerShoot() {

        RayHitInfo rayHitInfo = new RayHitInfo();

        RaycastHit2D rayHit = Physics2D.Raycast(barrelPoint.transform.position, transform.right, shotInfo.distance);
        if (rayHit) {
            GameObject hit = rayHit.collider.gameObject;

            //if gunfish
            if (hit.CompareTag("Gunfish")) {
                rayHitInfo.netId = hit.GetComponentInParent<Gunfish>().netId;
                rayHitInfo.color = Color.red;
                rayHitInfo.hitType = HitType.Fish;
            }

            //if generic object
            else if (hit.CompareTag("Ground")) {
                //rayHitInfo.netId.Value defaults to zero
                rayHitInfo.color = hit.gameObject.GetComponent<SpriteRenderer>().color;
                rayHitInfo.hitType = HitType.Wood;
            }

            rayHitInfo.normal = rayHit.normal;
            rayHitInfo.end = rayHit.point;           
        }
        else {
            //if nothing was hit
            rayHitInfo.netId = NetworkInstanceId.Invalid;
            rayHitInfo.end = barrelPoint.transform.position + (transform.right*shotInfo.distance);
        }

        rayHitInfo.origin = barrelPoint.transform.position;
        rayHitInfo.shotType = shotType;

        return rayHitInfo;
    }

    //Gunshot audio and visual fx
    public void DisplayShoot()
    {
        boomSound.Play();
        StartCoroutine(MuzzleFlash());
    }

    IEnumerator MuzzleFlash() {
        muzzleFlash.enabled = true;
        yield return flashDuration;
        muzzleFlash.enabled = false;
    }
}
