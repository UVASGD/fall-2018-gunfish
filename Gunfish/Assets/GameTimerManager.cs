using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimerManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        RaceManager.instance.InvokeGameTimer();
	}
}
