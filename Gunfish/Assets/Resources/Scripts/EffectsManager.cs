using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour {

    public static EffectsManager instance;

    [SerializeField]
    ParticleSystem[] debrisPool;
    int latestDP;

    [SerializeField]
    AudioSource[] audioPool;
    int latestAS;

    [SerializeField]
    LineRenderer[] linePool;
    int latestLR;
    WaitForSeconds shotDuration = new WaitForSeconds(0.04f);

	// Use this for initialization
	void Start () {
        if (!instance)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        //we'll probably want to preset these, since not every child that a particle system is necessarily part of the debris pool
        debrisPool = GetComponentsInChildren<ParticleSystem>();
        linePool = GetComponentsInChildren<LineRenderer>();
	}

    public void DisplayBulletHit(Vector2 point, Vector2 normal, Color color) {
        GameObject debris = debrisPool[latestDP].gameObject;
        debris.transform.position = point;
        debris.transform.up = normal;
        var main = debrisPool[latestDP].main;
        main.startColor = color;
        debrisPool[latestDP].Emit(10);

        latestDP = (latestDP+1) % debrisPool.Length;
    }

    //We'll want to make this dependent on the gun in question, but for now, just display this one type of line
    //We will include a variable for line thickness
    public void DisplayBulletLine(Vector2 origin, Vector2 end) {
        StartCoroutine(LineDisplay(linePool[latestLR], origin, end));
        latestLR = (latestLR+1) % linePool.Length;
    }

    IEnumerator LineDisplay(LineRenderer bulletLine, Vector2 origin, Vector2 end) {
        bulletLine.SetPosition(0, origin);
        bulletLine.SetPosition(1, end);
        bulletLine.enabled = true;
        yield return shotDuration;
        bulletLine.enabled = false;
    }
}
