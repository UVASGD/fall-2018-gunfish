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

    //This information will be included in a gun info ScriptableObject
    static float knockBackMagnitude = 1000f;
    
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
    private AudioSource flopSource;
    #endregion

    public void ApplyVariableDefaults () {
        maxJumpCD = 1f;
    }

    //Initialize Camera and audio sources for every local player
    public override void OnStartLocalPlayer () {

        MusicManager.instance.PlayMusic();

        Camera.main.GetComponent<Camera2DFollow>().target = transform;

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

        if (!rb)
            rb = GetComponent<Rigidbody2D>();

        if (!gun)
            gun = GetComponentInChildren<Gun>();

        groundedCount = 0;

        currentJumpCD = 0f;
        currentFireCD = 0f;
        currentAirborneJumpCD = 0f;

        transform.eulerAngles = Vector3.forward * 180f;

        //Set the maxFireCD of the gunfish to the gun's maxFireCD.
        //Fire cooldown is handled here to avoid multiple nested
        //Network Transforms
        maxFireCD = gun.maxFireCD;

        //Disable HingeJoints on all but the local player to
        //prevent weird desyncs in movement
        if (!isLocalPlayer) {
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
        if (isLocalPlayer) {
            ClientInputHandler();
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
                if (currentAirborneJumpCD <= 0f && transform.GetChild((transform.childCount / 2) - 1).GetComponent<Rigidbody2D>().angularVelocity < 360f) {
                    Rotate(100f * -x);
                }
            }
        }

        if (shoot && currentFireCD <= 0f) {
            Shoot();
        }
    }

    //Called on the client. Applies appropriate forces to the
    //Gunfish. To call this function appropriately, it should
    //be called from the server, after calculating what force
    //and torque should be applied from the server as well.
    public void Move (Vector2 force, float torque) {
        flopSource.clip = (flops.Length > 0 ? flops[Random.Range(0, flops.Length)] : null);
        flopSource.Play();

        transform.GetChild((transform.childCount / 2) - 1).GetComponent<Rigidbody2D>().AddForce(force);
        transform.GetChild((transform.childCount / 2) - 1).GetComponent<Rigidbody2D>().AddTorque(torque);

        currentJumpCD = maxJumpCD;

        //We might want to send a FlopMessage (for loud flops) so that everyone can hear the fish squish
    }

    public void Rotate (float torque) {
        transform.GetChild((transform.childCount / 2) - 1).GetComponent<Rigidbody2D>().AddTorque(torque);

        currentAirborneJumpCD = maxAirborneJumpCD;
    }

    //Called on the Client. Takes the info from the attached Gun
    //component of a child GameObject, and applies a force. If
    //there is no Gun attached, simply will not fire.
    public void Shoot () {
        rb.AddForceAtPosition(transform.right * gun.Force, transform.position);

        currentFireCD = maxFireCD;

        NetworkManager.singleton.client.Send(MessageTypes.GUNSHOT, new GunfishMsg(netId));
    }

    //Checks to see if any Transform in the Gunfish hierarchy
    //is touching the ground.
    public bool IsGrounded () {
        return (groundedCount == 0);
    }

    public void Knockback(Vector2 direction, Vector2 position) {
        rb.AddForceAtPosition(direction * knockBackMagnitude, position);
    }

    public void Hit(Vector2 direction, Vector2 position) {
        Knockback(direction, position);
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
