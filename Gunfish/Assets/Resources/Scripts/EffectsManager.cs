using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour {

    public static EffectsManager instance;

    public float shotSpeed = 800; //units/sec
    public Gradient defaultShotColor; // currently i can't find a way to set this in editor

    [SerializeField]
    ParticleSystem[] debrisPool;
    int latestDP;

    [SerializeField]
    AudioSource[] audioPool;
    int latestAS;

    public AudioClip[] FishClips;

    public AudioClip[] WoodClips;

    [SerializeField]
    LineRenderer[] linePool;
    int latestLR;
    //WaitForSeconds shotDuration = new WaitForSeconds(0.04f);

	// Use this for initialization
	void Awake () {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        //we'll probably want to preset these, since not every child that a particle system is necessarily part of the debris pool
        debrisPool = GetComponentsInChildren<ParticleSystem>();
        linePool = GetComponentsInChildren<LineRenderer>();

        // Configure Lines
        Material lineMaterial = new Material(Shader.Find("Particles/Additive"));
        foreach (LineRenderer line in linePool) {
            line.material = lineMaterial;
            line.colorGradient = defaultShotColor;
        }
        DontDestroyOnLoad(this);
	}

    public void DisplayBulletHit(Vector2 point, Vector2 normal, Color color, HitType hit){
        GameObject debris = debrisPool[latestDP].gameObject;
        
        debris.transform.position = point;
        debris.transform.up = normal;
        var main = debrisPool[latestDP].main;
        main.startColor = color;
        debrisPool[latestDP].Emit(10);
        latestDP = (latestDP+1) % debrisPool.Length;
        PlayHit(point, hit);
    }

    void PlayHit(Vector2 point, HitType hit) {
        GameObject audioPlayer = audioPool[latestAS].gameObject;
        audioPlayer.transform.position = point;
        switch (hit)
        {
            case HitType.Wood:
                audioPool[latestAS].clip = (WoodClips.Length > 0) ? WoodClips[Random.Range(0, WoodClips.Length)] : null;
                break;
            case HitType.Fish:
                audioPool[latestAS].clip = (FishClips.Length > 0) ? FishClips[Random.Range(0, FishClips.Length)] : null;
                break;
        }
        latestAS = (latestAS + 1) % audioPool.Length;
    }

    //We'll want to make this dependent on the gun in question, but for now, just display this one type of line
    //We will include a variable for line thickness
    public void DisplayBulletLine(Vector2 origin, Vector2 end) { // not in use
        linePool[latestLR].colorGradient = defaultShotColor;
        StartCoroutine(LineDisplay(linePool[latestLR], origin, end));
        latestLR = (latestLR+1) % linePool.Length;
    }
    public void DisplayBulletLine(Vector2 origin, Vector2 end, Gradient color) {
        linePool[latestLR].colorGradient = color;
        StartCoroutine(LineDisplay(linePool[latestLR], origin, end));
        latestLR = (latestLR+1) % linePool.Length;
    }

    IEnumerator LineDisplay(LineRenderer bulletLine, Vector2 origin, Vector2 end) {
        Ray ray = new Ray(origin, end - origin);
        float dist = Vector3.Distance(origin, end);
        float endTime = Time.time + Mathf.Min(dist/shotSpeed + 0.02f, 2); // a bullet will not display longer than 2 sec
        bulletLine.enabled = true;
        float pos = 0;
        while (Time.time < endTime) {
            bulletLine.SetPosition(0, ray.GetPoint(Mathf.Min(pos + 5.0f, dist)));
            pos = Mathf.Min(pos + shotSpeed*Time.deltaTime, dist);
            bulletLine.SetPosition(1, ray.GetPoint(pos));
            yield return null;
        }
        bulletLine.enabled = false;
    }
}
