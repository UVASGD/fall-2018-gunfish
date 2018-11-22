using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour {

    Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        StartCoroutine(StartCountdown());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator StartCountdown () {
        int secondsLeft = 3;

        while (secondsLeft > 0) {
            text.text = secondsLeft.ToString();
            secondsLeft--;
            yield return new WaitForSeconds(1f);
        }

        text.text = "GUNFISH!";
        yield return new WaitForSeconds(1f);
        text.text = "";
    }
}
