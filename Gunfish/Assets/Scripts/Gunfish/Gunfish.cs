//Gunfish.cs
//Written by Ryan Kann
//
//Purpose:
//Handle both server and client side properties of a Gunfish GameObject.
//Ex: User input, ground collision checks, applying physics forces
//
//How to Use:
//Make sure any Gunfish GameObject has this as a component. There is
//a Gunfish Creator Editor Window that helps with this. For existing
//Gunfish, you can simply change the public/serialized variables in
//the Inspector window.
//
//TODO:
//This script utilizes SmoothSync to function. Many methods are
//called on the client, but linked properly to the server and other
//clients inherently from the SmoothSyncs.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkConnection))]
[RequireComponent(typeof(Rigidbody2D))]
public class Gunfish : NetworkBehaviour {
    
    #region VARIABLES
    [Header("Input")]
    [SyncVar] public float currentJumpCD;
    [Range(0.1f, 5f)] public float maxJumpCD = 1f;
    [SyncVar] public float currentAirborneJumpCD;
    [Range(0.1f, 5f)] public float maxAirborneJumpCD = 0.4f;
    public bool fire;
    [SyncVar] [HideInInspector] public float currentFireCD;
    [HideInInspector] public float maxFireCD = 1f;

    [Header("Fish Info")]
    public Rigidbody2D rb;
    public Rigidbody2D middleRb;
    public Gun gun;

    [Tooltip("The number of fish pieces not touching the ground. (0 = grounded)")]
    public int groundedCount = 0;

    [Tooltip("This is the force and torque of the moving fish")]
    public float moveForce = 500f;
    public float moveTorque = 100f;

    [Header("Audio")]
    public AudioClip[] flops;
    private AudioSource flopSource;
    #endregion

    public void ApplyVariableDefaults () {
        maxJumpCD = 1f;
    }

    //public override void OnStartAuthority() {
    //    rb = GetComponent<Rigidbody2D>();
    //    rb.isKinematic = false;
    //}

    //Initialize Camera and audio sources for ever local player
    public override void OnStartLocalPlayer () {
        MusicManager.instance.PlayMusic();

        if (Camera.main.GetComponent<Camera2DFollow>() != null) {
            Camera.main.GetComponent<Camera2DFollow>().target = transform;
        }

        //Setup the local audio handlers
        /***********************************************************/
        //Flop sounds
        if (GetComponent<AudioSource>()) {
            flopSource = gameObject.GetComponent<AudioSource>();
        } else {
            flopSource = gameObject.AddComponent<AudioSource>();
        }

        if (flops.Length == 0) {
            flops = Resources.LoadAll<AudioClip>("Audio/Flops/");
        }

        flopSource.clip = (flops.Length > 0 ? flops[Random.Range(0, flops.Length)] : null);
        /***********************************************************/
    }

    //When the Gunfish is started (server and client), assign fish info
    private void Start () {
        if (isServer || hasAuthority) {
            if (!rb)
                rb = GetComponent<Rigidbody2D>();

            if (!gun)
                gun = GetComponentInChildren<Gun>();

            middleRb = transform.GetChild((transform.childCount / 2) - 1).GetComponent<Rigidbody2D>();

            groundedCount = 0;

            currentJumpCD = 0f;
            currentFireCD = 0f;
            currentAirborneJumpCD = 0f;

            transform.eulerAngles = Vector3.forward * 180f;

            //Set the maxFireCD of the gunfish to the gun's maxFireCD.
            //Fire cooldown is handled here to avoid multiple nested
            //Network Transforms
            maxFireCD = gun.shotInfo.maxFireCD;
        }

        //Disable HingeJoints on all but the local player to
        //prevent weird desyncs in movement
        if (!hasAuthority) {
            rb.bodyType = RigidbodyType2D.Kinematic;
            foreach (Transform child in transform) {

                if (child.GetComponent<Rigidbody2D>()) {
                    child.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                }
            }
        }
    }

    //Calls input handler on appropriate local players.
    //Also handles cooldowns
    private void Update () {
        if (hasAuthority) {
            ClientInputHandler();
        }

        CDUpdate(ref currentJumpCD, maxJumpCD);
        CDUpdate(ref currentFireCD, maxFireCD);
        CDUpdate(ref currentAirborneJumpCD, maxAirborneJumpCD);

        if (groundedCount < 0) {
            groundedCount = 0;
        }
    }

    private void CDUpdate(ref float CD, float maxCD, bool blocking = false) {
        if (float.IsNaN(CD)) {
            return;
        }

        if (CD > maxCD) {
            CD = maxCD;
            if (blocking) {
                //increment isBlocking
            }
        }
        else if (CD > 0f)
            CD -= Time.deltaTime;
        else {
            if (blocking) {
                //decrement isBlocking;
            }
            CD = float.NaN;
        }
    }

    //Checks user input on the Client. Also returns whether
    //or not an input message should be sent to the server.
    public void ClientInputHandler () {
        float x = Input.GetAxisRaw("Horizontal");
        bool shoot = Input.GetButtonDown("Fire1");

        bool apply = (x != 0f|| shoot);

        if (apply) {
            ApplyMovement(x, shoot);
        }
    }

    //If the movement is non-zero, apply it. Since Gunfish
    //Utilizes NetworkTransforms, this automatically syncs
    //to the server as well as every client
    public void ApplyMovement (float x, bool shoot) {
        if (x != 0) {
            if (groundedCount > 0) {
                if (float.IsNaN(currentJumpCD)) {
                    Move(new Vector2(x, 1f).normalized * moveForce, -x * moveForce * Random.Range(0.5f, 1f));
                }
            } else {
                if (float.IsNaN(currentAirborneJumpCD) && middleRb.angularVelocity < 360f) {
                    Rotate(moveTorque * -x);
                }
            }
        }

        if (shoot && float.IsNaN(currentFireCD)) {
            Shoot();
        }
    }

    //Called on the client. Applies appropriate forces to the
    //Gunfish. To call this function appropriately, it should
    //be called from the server, after calculating what force
    //and torque should be applied from the server as well.
    public void Move (Vector2 force, float torque) {
        flopSource.clip = (flops.Length > 0) ? flops[Random.Range(0, flops.Length)] : null;
        flopSource.Play();

        middleRb.AddForce(force);
        middleRb.AddTorque(torque);

        currentJumpCD = maxJumpCD;

        //We might want to send a FlopMessage (for loud flops) so that everyone can hear the fish squish
    }

    public void Rotate (float torque) {
        middleRb.AddTorque(torque);

        currentAirborneJumpCD = maxAirborneJumpCD;
    }

    //Called on the Client. Takes the info from the attached Gun
    //component of a child GameObject, and applies a force. If
    //there is no Gun attached, simply will not fire.
    public void Shoot () {
        rb.AddForceAtPosition(transform.right * gun.shotInfo.force, transform.position);

        currentFireCD = maxFireCD;

        NetworkManager.singleton.client.Send(MessageTypes.GUNSHOT, new GunfishMsg(netId));
    }

    //Checks to see if any Transform in the Gunfish hierarchy
    //is touching the ground.
    public bool IsGrounded () {
        return (groundedCount == 0);
    }

    public void Knockback(Vector2 direction, ShotType shotType) {
        rb.AddForce(direction * Misc.ShotDict[shotType].knockbackMagnitude);
    }

    public void Hit(Vector2 direction, ShotType shotType) {
        Knockback(direction, shotType);
        //Check gamemode, if race, then call Stun(), else call Damage()
    }

    public void DisplayShoot() {
        gun.DisplayShoot();
    }


    //SERVER CALLBACKS
    [ServerCallback]
    public RayHitInfo ServerShoot() {
        return gun.ServerShoot();
    }

}
