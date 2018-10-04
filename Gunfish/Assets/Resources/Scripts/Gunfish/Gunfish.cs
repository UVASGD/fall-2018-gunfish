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
//This script utilizes NetworkTransforms to function. Many methods are
//called on the client, but linked properly to the server and other
//clients inherently from the NetworkTransforms.
//
//I actually really don't like how this is set up for two reasons.
//1) Network transforms are leave a lot of syncing under the hood, so
//   it's much harder to figure out how pieces of code work, and whether
//   they are called on the Client or the Server.
//2) NetworkTransforms are actually not very good at handling multiple
//   layers of chained Transforms, and the Gunfish are really jittery.
//   A potential fix for this I'm experimenting with is seeing disabling
//   various physics attributes on Gunfish without local player authority.

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
    public Gun gun;

    [Tooltip("The number of fish pieces not touching the ground. (0 = grounded)")]
    public int groundedCount = 0;

    [Header("Audio")]
    public AudioClip[] flops;
    public AudioClip[] shots;

    private AudioSource flopSource;
    private AudioSource shotSource;

    private GameObject debris;
    #endregion

    public void ApplyVariableDefaults () {
        maxJumpCD = 1f;

    }

    public override void OnStartAuthority () {
        rb.isKinematic = false;
    }

    //Initialize Camera and audio sources for ever local player
    public override void OnStartLocalPlayer () {
        base.OnStartLocalPlayer();

        MusicManager.instance.PlayMusic();

        Camera.main.GetComponent<Camera2DFollow>().target = transform;

        //Ensure that the gun is referenced before depending on it
        if (!gun) {
            gun = GetComponentInChildren<Gun>();
        }

        if (debris == null) {
            debris = Resources.Load<GameObject>("Prefabs/Debris");
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

        //Shot sounds
        if (gun.GetComponent<AudioSource>()) {
            shotSource = gun.gameObject.GetComponent<AudioSource>();
        } else {
            shotSource = gun.gameObject.AddComponent<AudioSource>();
        }
        if (shots.Length == 0) {
            shots = new AudioClip[] {Resources.LoadAll<AudioClip>("Audio/Shots/")[0]};
        }

        shotSource.clip = (shots.Length > 0 ? shots[Random.Range(0, shots.Length)] : null);
        /***********************************************************/
    }

    //When the Gunfish is started (server and client), assign fish info
    private void Start () {
        //Register Messages
        NetworkManager.singleton.client.RegisterHandler(MessageTypes.GUNSHOTHITMSG, OnGunshotHit);

        rb = GetComponent<Rigidbody2D>();
        gun = GetComponentInChildren<Gun>();

        groundedCount = 0;

        currentJumpCD = 0f;
        currentFireCD = 0f;
        currentAirborneJumpCD = 0f;

        transform.eulerAngles = Vector3.forward * 180f;

        //Set tha maxFireCD of the gunfish to the gun's maxFireCD.
        //Fire cooldown is handled here to avoid multiple nested
        //Network Transforms
        maxFireCD = gun.maxFireCD;

        //Disable HingeJoints on all but the local player to
        //prevent weird desyncs in movement
        if (!isLocalPlayer) {
            rb.bodyType = RigidbodyType2D.Kinematic;
            foreach (Transform child in transform) {
                //if (child.GetComponent<HingeJoint2D>()) {
                //    child.GetComponent<HingeJoint2D>().enabled = false;
                //}

                if (child.GetComponent<Rigidbody2D>()) {
                    child.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                }
            }
        }
    }

    //Calls input handler on appropriate local players.
    //Also handles cooldowns
    private void Update () {
        if (isLocalPlayer) {
            ClientInputHandler();
            //Debug.Log("Grounded: " + (groundedCount > 0));
        }

        if (currentJumpCD <= 0f) {
            currentJumpCD = 0f;
        } else {
            currentJumpCD -= Time.deltaTime;
        }

        if (currentFireCD <= 0f) {
            currentFireCD = 0f;
        } else {
            currentFireCD -= Time.deltaTime;
        }

        if (currentAirborneJumpCD <= 0f) {
            currentAirborneJumpCD = 0f;
        } else {
            currentAirborneJumpCD -= Time.deltaTime;
        }

        if (groundedCount < 0) {
            groundedCount = 0;
        }
    }

    //Checks user input on the Client. Also returns whether
    //or not an input message should be sent to the server.
    public void ClientInputHandler () {
        float x = Input.GetAxisRaw("Horizontal");
        bool shoot = Input.GetKeyDown(KeyCode.Space);

        bool apply = (x != 0f || shoot);

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
                if (currentJumpCD <= 0f) {
                    Move(new Vector2(x, 1f).normalized * 500f, -x * 500f * Random.Range(0.5f, 1f));
                }
            } else {
                if (currentAirborneJumpCD <= 0f && transform.GetChild(transform.childCount / 2).GetComponent<Rigidbody2D>().angularVelocity < 360f) {
                    Rotate(50f * -x);
                }
            }
        }

        if (shoot && currentFireCD <= 0f) {
            Shoot();
        }
    }

    //Called on the server. Applies appropriate forces to the
    //Gunfish. To call this function appropriately, it should
    //be called from the server, after calculating what force
    //and torque should be applied from the server as well.
    //[ServerCallback]
    public void Move (Vector2 force, float torque) {
        flopSource.clip = (flops.Length > 0 ? flops[Random.Range(0, flops.Length)] : null);
        flopSource.Play();

        transform.GetChild(transform.childCount / 2).GetComponent<Rigidbody2D>().AddForce(force);
        transform.GetChild(transform.childCount / 2).GetComponent<Rigidbody2D>().AddTorque(torque);

        currentJumpCD = maxJumpCD;
    }

    public void Rotate (float torque) {
        transform.GetChild(transform.childCount / 2).GetComponent<Rigidbody2D>().AddTorque(torque);

        currentAirborneJumpCD = maxAirborneJumpCD;
    }

    //Called on the server. Takes the info from the attached Gun
    //component of a child GameObject, and applies a force. If
    //there is no Gun attached, simply will not fire.
    public void Shoot () {
        rb.AddForceAtPosition(transform.right * gun.force, transform.position);

        currentFireCD = maxFireCD;

        Vector3 position = transform.position;

        NetworkManager.singleton.client.Send(MessageTypes.GUNSHOTAUDIOMSG, new GunshotAudioMsg(0, position));
        NetworkManager.singleton.client.Send(MessageTypes.NETIDMSG, new NetIdMsg(netId));

        ////Shoot bullet raycast. Do multiple hits to avoid collision with own fish pieces
        //Vector3 direction = transform.GetChild(transform.childCount-1).right.normalized;
        //RaycastHit2D[] hits = Physics2D.RaycastAll(
        //    transform.GetChild(transform.childCount-1).position, direction
        //);

        //if (hits.Length > 0) {
        //    foreach (RaycastHit2D hit in hits) {
        //        if (hit.collider.CompareTag("Player")) {
        //            //If it's a fish, ensure it's applied to the root
        //            Rigidbody2D target = null;
        //            if (hit.transform.parent == null) {
        //                target = hit.transform.GetComponent<Rigidbody2D>();
        //            } else if (hit.transform.parent.CompareTag("Player")) {
        //                target = hit.transform.parent.GetComponent<Rigidbody2D>();
        //            }

        //            if (target != null) {
        //                target.AddForce(direction * 200f);
        //            }

        //        } else if (hit.collider.CompareTag("Ground")) {
        //            GameObject hitDebris = Instantiate<GameObject>(debris, hit.point, Quaternion.Euler(hit.normal));
        //            Destroy(hitDebris, 2f);
        //        }
        //    }
        //}
    }

    //Checks to see if any Transform in the Gunfish hierarchy
    //is touching the ground.
    public bool IsGrounded () {
        return (groundedCount == 0);
    }

    #region MESSAGE HANDLERS

    private void OnGunshotHit (NetworkMessage netMsg) {
        GunshotHitMsg msg = netMsg.ReadMessage<GunshotHitMsg>();

        if (rb) {
            rb.AddForceAtPosition(msg.force, msg.position);
        }

        //TODO
        //LOSE HEALTH HERE
        //health -= msg.damage;
    }

    #endregion
}
