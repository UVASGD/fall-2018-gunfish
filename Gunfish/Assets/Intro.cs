using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour {

    private Transform text;
    public float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("Pos: " + Camera.main.WorldToScreenPoint(transform.position));
        Vector3 target = Vector3.zero;
        target.x = transform.position.x + speed * SpeedMultiplier();

        transform.position = target;
	}

    float SpeedMultiplier () {
        int width = Screen.width;
        int span = width / 2;
        float x = transform.position.x - width;
        //Debug.Log("Width: " + width + ", x: " + x);

        float result = 20 * (0.2f - span / (x * x + span * span));
        Debug.Log("x: " + x + ", " + "y: " + result);
        return result;
    }
}
