using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleSpawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        foreach (Transform child in transform) {
            //child.localPosition = Vector3.zero;
        }
        StartCoroutine(Spawn());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Spawn () {
        foreach (Transform child in transform) {
            child.localPosition = Vector3.zero;
            yield return new WaitForSeconds(5f);
        }
    }
}
