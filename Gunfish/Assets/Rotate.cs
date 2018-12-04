using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    public float degreesPerSecond = 90f;
    public bool allowRotation = false;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward, degreesPerSecond * Time.deltaTime);

        if (!allowRotation) {
            foreach (Transform child in transform) {
                child.rotation = Quaternion.identity;
            }
        }
	}
}
