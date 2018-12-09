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
    //static float Misc.knockBackMagnitude = 1000f;

    #region VARIABLES
    [Header("Input")]
    public float currentJumpCD;
    [Range(0.1f, 5f)] public float maxJumpCD = 1f;
    public float currentAirborneJumpCD;
    [Range(0.1f, 5f)] public float maxAirborneJumpCD = 0.4f;
    public float currentSwimCD;
    [Range(0.1f, 5f)] public float maxSwimCD = 0.25f;
    public bool fire;
    [HideInInspector] public float currentFireCD;
    [HideInInspector] public float maxFireCD = 1f;
    [HideInInspector]
    float currentStunCD = float.NaN;
    public int isBlocked = 0;

    [Header("Movement")]
    public float thrustForce = 200f;
    public float swimTorque = 175f;
    public float jumpForce = 500f;
    public float moveTorque = 100f;
    int isSwimming = 0;

    public ShotType shotType = ShotType.Medium;
    public bool auto = false;
    public float autoTorqueMulti = 0.01f;

    [Header("Fish Info")]
    public Rigidbody2D rb;
    public Rigidbody2D middleRb;
    public Gun gun;
  
    [Tooltip("The number of fish pieces not touching the ground. (0 = grounded)")]
    public int groundedCount = 0;

    [Header("Audio")]
    public AudioClip[] flops;
    private AudioSource flopSource;

    [Header("Nameplate")]
    public GameObject nameplatePrefab;
    NamePlate nameplate;
    [SyncVar]
    public string gameName;

    [SyncVar]
    public bool crowned = false;
    #endregion

    public void ApplyVariableDefaults() {
        maxJumpCD = 1f;
    }

    //Initialize Camera and audio sources for every local player
    public override void OnStartLocalPlayer() {
        Camera.main.GetComponent<Camera2DFollow>().target = transform;

        //Setup the local audio handlers
        /***********************************************************/
        //Flop sounds
        if (GetComponent<AudioSource>()) {
            flopSource = gameObject.GetComponent<AudioSource>();
        }
        else {
            flopSource = gameObject.AddComponent<AudioSource>();
        }

        if (flops.Length == 0) {
            flops = Resources.LoadAll<AudioClip>("Audio/Flops/");
        }

        flopSource.clip = (flops.Length > 0 ? flops[Random.Range(0, flops.Length)] : null);
        /***********************************************************/
    }

    //When the Gunfish is started (server and client), assign fish info
    private void Start() {

        if (isServer || hasAuthority) {
            if (!rb)
                rb = GetComponent<Rigidbody2D>();

            if (!gun)
                gun = GetComponentInChildren<Gun>();

            gun.SetShotType(shotType);

            groundedCount = 0;

            currentJumpCD = 0f;
            currentFireCD = 0f;
            currentAirborneJumpCD = 0f;

            transform.eulerAngles = Vector3.forward * 180f;

            //Set the maxFireCD of the gunfish to the gun's maxFireCD.
            //Fire cooldown is handled here to avoid multiple nested
            //Network Transforms
            maxFireCD = gun.shotInfo.maxFireCD;

            if (hasAuthority) {
                PlayerController.ownedGunfish = this;
            }
        }

        middleRb = transform.GetChild((transform.childCount / 2) - 1).GetComponent<Rigidbody2D>();
        GameObject nameplateObj = Instantiate(nameplatePrefab, transform.position, Quaternion.identity);
        nameplate = nameplateObj.GetComponent<NamePlate>();
        nameplate.SetOwner(middleRb.gameObject);
        SetName(gameName);

        if (crowned)
            Crowner.SpawnCrown(gameObject);

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
    private void Update() {
        if (isLocalPlayer) {
            if (isBlocked == 0) {
                ClientInputHandler();
            }
            CheckCoolDowns();
        }
        // int latency = NetworkManager.singleton.client.GetRTT();
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
        }
        else if (CD > 0f)
            CD -= Time.deltaTime;
        else {
            if (blocking) {
                isBlocked--;
            }
            CD = float.NaN;
        }
    }

    private void CheckCoolDowns() {
        CDUpdate(ref currentJumpCD, maxJumpCD);
        CDUpdate(ref currentFireCD, maxFireCD);
        CDUpdate(ref currentAirborneJumpCD, maxAirborneJumpCD);
        CDUpdate(ref currentSwimCD, maxSwimCD);
        CDUpdate(ref currentStunCD, float.PositiveInfinity, true);
    }

    //Checks user input on the Client. Also returns whether
    //or not an input message should be sent to the server.
    public void ClientInputHandler() {
        float x = Input.GetAxisRaw("Horizontal");
        bool shoot = auto ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
        // ternary operator ?: if (auto) =statement1 else =statement2

        bool apply = (x != 0f || shoot);

        if (apply) {
            ApplyMovement(x, shoot);
        }

        //Change your fish (Lobby only)
        if (Input.GetKeyDown(KeyCode.M) && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Lobby")) {
            //call server
            NetworkManager.singleton.client.Send(MessageTypes.CHANGEFEEESH, new GunfishMsg(netId));
        }
    }

    public void ChangeFeesh() {
        //print("Scene Name: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        List<GameObject> fishList = new List<GameObject>(GunfishList.Get());
        GetComponent<Collider2D>().enabled = false;

        int index = Random.Range(0, fishList.Count); // initial number when you connect to server for first time

        if (RaceManager.instance.fishTable.ContainsKey(connectionToClient))
        {
            index = (RaceManager.instance.fishTable[connectionToClient] + 1) % fishList.Count;
        }

        ChangeFeesh(index);
    }

    public void ClientChangeFeesh (int index) {
        NetworkManager.singleton.client.Send(MessageTypes.CHANGEFEEESH, new GunfishSelectMsg(netId, index));
    }

    public void ChangeFeesh(int index) {
        List<GameObject> fishList = new List<GameObject>(GunfishList.Get());
        //print("s:" + fishList.Count);
        GameObject newFish = Instantiate(fishList[index], transform.position, transform.rotation) as GameObject;
        newFish.GetComponent<LineRenderer>().enabled = false;
        newFish.GetComponent<Gunfish>().gameName = gameName;

        Rigidbody2D myRb = GetComponent<Rigidbody2D>();
        Rigidbody2D otherRb = newFish.GetComponent<Rigidbody2D>();

        if (RaceManager.instance.fishTable.ContainsKey(connectionToClient))
        {
            RaceManager.instance.fishTable[connectionToClient] = index;
        }

        otherRb.position = myRb.position;
        otherRb.rotation = myRb.rotation;
        otherRb.velocity = myRb.velocity;
        otherRb.angularVelocity = myRb.angularVelocity;

        NetworkServer.ReplacePlayerForConnection(connectionToClient, newFish, playerControllerId);
        NetworkServer.Destroy(gameObject);

        newFish.GetComponent<LineRenderer>().enabled = true;
    }

    public override void OnNetworkDestroy()
    {
        if (nameplate) {
            Destroy(nameplate.gameObject);
        }
    }

    //If the movement is non-zero, apply it. Since Gunfish
    //Utilizes NetworkTransforms, this automatically syncs
    //to the server as well as every client
    public void ApplyMovement(float x, bool shoot) {
        if (x != 0) {
            if (groundedCount > 0 && isSwimming == 0) {
                if (float.IsNaN(currentJumpCD)) {
                    Move(new Vector2(x, 1f).normalized * jumpForce, -x * jumpForce * Random.Range(0.5f, 1f));
                }
            }
            else {
                if (float.IsNaN(currentAirborneJumpCD) && middleRb.angularVelocity < 360f) {
                    if (isSwimming > 0) {
                        Rotate(swimTorque * -x);
                    }
                    else {
                        Rotate(moveTorque * -x);
                    }
                }
            }
        }

        if (shoot && float.IsNaN(currentFireCD)) {
            if (isSwimming > 0) {
                Thrust();
            }
            else {
                Shoot();
            }
        }
    }

    //Called on the client. Applies appropriate forces to the
    //Gunfish. To call this function appropriately, it should
    //be called from the server, after calculating what force
    //and torque should be applied from the server as well.
    public void Move(Vector2 force, float torque) {
        if (flopSource) {
            flopSource.clip = (flops.Length > 0 ? flops[Random.Range(0, flops.Length)] : null);
            flopSource.Play();
        }

        middleRb.AddForce(force);
        middleRb.AddTorque(torque);

        currentJumpCD = maxJumpCD;

        //We might want to send a FlopMessage (for loud flops) so that everyone can hear the fish squish
    }

    public void Rotate(float torque) {
        middleRb.AddTorque(torque);

        currentAirborneJumpCD = maxAirborneJumpCD;
    }

    //Called on the Client. Takes the info from the attached Gun
    //component of a child GameObject, and applies a force. If
    //there is no Gun attached, simply will not fire.
    public void Shoot() {
        float shotForce = gun.shotInfo.force;
        rb.AddForceAtPosition(transform.right * shotForce, transform.position);
        if (auto) rb.AddTorque(shotForce*autoTorqueMulti*(Random.Range(0.4f, 0.1f)));

        currentFireCD = maxFireCD;

        NetworkManager.singleton.client.Send(MessageTypes.GUNSHOT, new GunfishMsg(netId));
    }

    //Checks to see if any Transform in the Gunfish hierarchy
    //is touching the ground.
    public bool IsGrounded() {
        return (groundedCount == 0);
    }

    public void Knockback(Vector2 direction, ShotType shotType) {
        rb.AddForce(direction * Misc.ShotDict[shotType].knockbackMagnitude);
    }

    public void Hit(Vector2 direction, ShotType shotType) {
        Knockback(direction, shotType);
        Stun(Misc.ShotDict[shotType].stunTime);
        //Check gamemode, if race, then call Stun(), else call Damage()
    }

    public void Stun(float stunTime) {
        //Oh no you got stunned
        if (float.IsNaN(currentStunCD))
            isBlocked++;
        currentStunCD = stunTime;
    }

    public void DisplayShoot() {
        gun.DisplayShoot();
    }


    public void Swim() {
        if (isSwimming == 0) {
            Rigidbody2D[] sliceBodies = GetComponentsInChildren<Rigidbody2D>();
            foreach (Rigidbody2D sliceRb in sliceBodies) {
                sliceRb.gravityScale = 0;
                sliceRb.angularDrag = 10;
            }

            isSwimming = 1;
        }
    }

    void Thrust() {
        if (float.IsNaN(currentSwimCD)) {
            rb.AddRelativeForce(new Vector2(-thrustForce, 0));
        }
    }

    public void Unswim() {
        if (isSwimming > 0) {
            Rigidbody2D[] sliceBodies = GetComponentsInChildren<Rigidbody2D>();
            foreach (Rigidbody2D sliceRb in sliceBodies) {
                sliceRb.gravityScale = 1;
                sliceRb.angularDrag = 0.05f;
            }
            
            isSwimming = 0;
        }
    }


    public void SetName(string newName) {
        Stun(3f);
        if (nameplate) {
            nameplate.SetName(newName);
        } else {
            print("Nameplate is null!");
        }
    }

 
    [ClientRpc]
    public void RpcSetName(string newName) {
        SetName(newName);
    }

    [ClientRpc]
    public void RpcCrown() {
        if (!crowned) {
            crowned = true;
            Crowner.SpawnCrown(gameObject);
        }
    }

    //SERVER CALLBACKS
    [ServerCallback]
    public RayHitInfo ServerShoot(Gunfish gunfish) {
        return gun.ServerShoot(gunfish);
    }

}
