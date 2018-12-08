using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[ExecuteInEditMode]
public class UnnetworkedLineSegment : MonoBehaviour {

    public UnnetworkedGunfish gunfish;
    public LineRenderer segment;
    public int index;
    public bool grounded;

    private void Start () {
        grounded = true; //Necessary to start grounded to give player control overself by default

        if ((gunfish = GetComponent<UnnetworkedGunfish>()) == null) {
            gunfish = transform.parent.GetComponent<UnnetworkedGunfish>();
        }
    }

    // Update is called once per frame
    void Update () {
        segment.SetPosition(index, transform.position);
	}

    private void OnCollisionEnter2D (Collision2D collision) {
        if (collision.collider.tag == "Ground" || collision.collider.tag == "Object") {
            //if (grounded == false) {
                if (gunfish != null) {
                    gunfish.groundedCount++;
                }
            //}
            grounded = true;
        }
    }

    private void OnCollisionExit2D (Collision2D collision) {
        if (collision.collider.tag == "Ground" || collision.collider.tag == "Object") {
            //if (grounded == true) {
                if (gunfish != null) {
                    gunfish.groundedCount--;
                }
            //}
            grounded = false;
        }
    }
}
