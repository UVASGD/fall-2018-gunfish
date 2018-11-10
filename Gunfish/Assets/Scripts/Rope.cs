using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {
    public LineRenderer lr;
    public GameObject hookSpot;
    public GameObject originalSpot;
	// Use this for initialization
	void Start () {
        lr.SetPosition(0, originalSpot.transform.position);
        lr.SetPosition(1, hookSpot.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
        lr.SetPosition(1,hookSpot.transform.position);
	}
}
